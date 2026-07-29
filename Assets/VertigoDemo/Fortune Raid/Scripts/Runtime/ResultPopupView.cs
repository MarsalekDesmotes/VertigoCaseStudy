using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class ResultPopupView : MonoBehaviour
    {
        private const float PopupFadeDuration = 0.16f;
        private const float PopupCloseDuration = 0.13f;
        private const float RewardCountDuration = 0.42f;

        [SerializeField] private RectTransform ui_transform_popup_animator;
        [SerializeField] private CanvasGroup ui_canvas_group_result;
        [SerializeField] private Image ui_image_popup_panel_value;
        [SerializeField] private Image ui_image_result_card_value;
        [SerializeField] private Image ui_image_result_card_glow_value;
        [SerializeField] private Image ui_image_result_card_border_value;
        [SerializeField] private TMP_Text ui_text_result_title_value;
        [SerializeField] private TMP_Text ui_text_result_message_value;
        [SerializeField] private TMP_Text ui_text_result_reward_value;
        [SerializeField] private Image ui_image_result_icon_value;
        [SerializeField] private RewardFlyView ui_reward_fly_view;
        [SerializeField] private Button ui_button_result_primary;
        [SerializeField] private TMP_Text ui_text_result_primary_value;
        [SerializeField] private GameObject ui_panel_bomb_actions;
        [SerializeField] private Sprite ui_sprite_result_speed_lines;
        [SerializeField] private Sprite ui_sprite_result_special_shine;

        private Action primaryAction;
        private Sequence resultTween;
        private Tween resultCountTween;
        private RectTransform pendingRewardFlyTarget;
        private Sprite pendingRewardFlySprite;

        internal RectTransform Animator => ui_transform_popup_animator;
        internal TMP_Text Title => ui_text_result_title_value;
        internal TMP_Text Message => ui_text_result_message_value;
        internal Image Icon => ui_image_result_icon_value;
        internal Image Overlay => GetComponent<Image>();

        private void Awake()
        {
            ui_button_result_primary.onClick.AddListener(HandlePrimaryPressed);
        }

        private void OnDestroy()
        {
            resultTween.Kill(false);
            resultCountTween.Kill(false);
            ui_button_result_primary.onClick.RemoveListener(HandlePrimaryPressed);
        }

        public void ShowReward(
            RewardDefinition reward,
            int amount,
            RectTransform flyTarget,
            Action continueAction)
        {
            bool special = reward.IsSpecial;
            Open(
                special ? "SPECIAL REWARD!" : "REWARD SECURED!",
                special
                    ? "Limited reward added to your run."
                    : "Push your luck in the next zone.",
                reward.DisplayName + "  x" + amount,
                reward.Icon,
                "CONTINUE",
                continueAction,
                new Color(0.95f, 0.50f, 0.04f),
                new Color(0.08f, 0.055f, 0.02f, 0.99f),
                new Color(1f, 0.54f, 0.04f, 1f));

            if (special)
            {
                ui_image_result_card_glow_value.sprite = ui_sprite_result_special_shine;
                ui_text_result_reward_value.color = new Color(1f, 0.82f, 0.24f);
            }

            pendingRewardFlyTarget = flyTarget;
            pendingRewardFlySprite = reward.Icon;
            int displayedAmount = 0;
            ui_text_result_reward_value.text = reward.DisplayName + "  x0";
            resultCountTween.Kill(false);
            resultCountTween = DOTween.To(
                    () => displayedAmount,
                    value =>
                    {
                        displayedAmount = value;
                        ui_text_result_reward_value.text =
                            reward.DisplayName + "  x" + displayedAmount;
                    },
                    amount,
                    RewardCountDuration)
                .SetDelay(0.30f)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .SetTarget(ui_text_result_reward_value);
        }

        public void ShowCollected(
            Sprite chestIcon,
            int rewardKinds,
            Action restartAction)
        {
            Open(
                "REWARDS COLLECTED",
                "You safely left with your rewards.",
                rewardKinds + " REWARD TYPES",
                chestIcon,
                "NEW RUN",
                restartAction,
                new Color(0.16f, 0.50f, 0.76f),
                new Color(0.02f, 0.05f, 0.085f, 0.99f),
                new Color(1f, 0.64f, 0.08f, 1f));
        }

        internal Sequence ShowBombFrame(Sprite bombIcon)
        {
            ResetTransientState();
            Configure(
                "OH NO, A BOMB EXPLODED RIGHT IN YOUR HANDS!",
                "Revive yourself to keep your rewards.",
                string.Empty,
                bombIcon,
                string.Empty,
                new Color(0.72f, 0.12f, 0.08f),
                new Color(0.08f, 0.015f, 0.02f, 0.99f),
                new Color(0.86f, 0.08f, 0.04f, 1f),
                true);

            Image overlay = Overlay;
            resultTween.Kill(false);
            resultTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(gameObject)
                .Append(DOTween.To(
                    () => ui_canvas_group_result.alpha,
                    value => ui_canvas_group_result.alpha = value,
                    1f,
                    PopupFadeDuration))
                .Join(ui_transform_popup_animator
                    .DOScale(new Vector3(1.04f, 1.04f, 1f), 0.22f)
                    .SetEase(Ease.OutBack))
                .Append(ui_transform_popup_animator
                    .DOScale(Vector3.one, 0.10f)
                    .SetEase(Ease.OutQuad))
                .Insert(0f, DOTween.To(
                    () => overlay.color,
                    value => overlay.color = value,
                    new Color(0f, 0f, 0f, 0.82f),
                    0.24f))
                .Insert(0.10f, ui_transform_popup_animator
                    .DOPunchRotation(new Vector3(0f, 0f, 2.4f), 0.30f, 12, 0.45f)
                    .SetEase(Ease.InOutSine))
                .Insert(0.08f, DOTween.To(
                    () => ui_text_result_title_value.alpha,
                    value => ui_text_result_title_value.alpha = value,
                    1f,
                    0.12f))
                .Insert(0.08f, ui_text_result_title_value.rectTransform
                    .DOScale(Vector3.one, 0.16f)
                    .SetEase(Ease.OutBack))
                .Insert(0.15f, DOTween.To(
                    () => ui_text_result_message_value.alpha,
                    value => ui_text_result_message_value.alpha = value,
                    1f,
                    0.12f))
                .Insert(0.18f, ui_image_result_icon_value.rectTransform
                    .DOScale(new Vector3(1.16f, 1.16f, 1f), 0.16f)
                    .SetEase(Ease.OutBack))
                .Insert(0.34f, ui_image_result_icon_value.rectTransform
                    .DOScale(Vector3.one, 0.08f)
                    .SetEase(Ease.OutQuad));
            return resultTween;
        }

        public void Hide()
        {
            ResetTransientState();
            gameObject.SetActive(false);
            ui_reward_fly_view.Hide();
        }

        internal void CloseAndInvoke(Action action)
        {
            RectTransform flyTarget = pendingRewardFlyTarget;
            Sprite flySprite = pendingRewardFlySprite;
            Vector3 flyStart = ui_image_result_icon_value.rectTransform.position;
            primaryAction = null;
            pendingRewardFlyTarget = null;
            pendingRewardFlySprite = null;
            ui_button_result_primary.interactable = false;
            resultCountTween.Kill(false);
            resultCountTween = null;
            resultTween.Kill(false);
            resultTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(gameObject)
                .Append(ui_transform_popup_animator
                    .DOScale(
                        new Vector3(0.9f, 0.9f, 1f),
                        PopupCloseDuration)
                    .SetEase(Ease.InBack))
                .Join(DOTween.To(
                    () => ui_canvas_group_result.alpha,
                    value => ui_canvas_group_result.alpha = value,
                    0f,
                    PopupCloseDuration))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    resultTween = null;
                    if (flyTarget != null && flySprite != null)
                    {
                        ui_reward_fly_view.Play(
                            flySprite,
                            flyStart,
                            flyTarget,
                            action);
                    }
                    else
                    {
                        action?.Invoke();
                    }
                });
        }

        private void Open(
            string title,
            string message,
            string reward,
            Sprite icon,
            string buttonLabel,
            Action action,
            Color buttonColor,
            Color panelColor,
            Color cardColor)
        {
            ResetTransientState();
            primaryAction = action;
            Configure(
                title,
                message,
                reward,
                icon,
                buttonLabel,
                buttonColor,
                panelColor,
                cardColor,
                false);

            resultTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(gameObject)
                .Append(DOTween.To(
                    () => ui_canvas_group_result.alpha,
                    value => ui_canvas_group_result.alpha = value,
                    1f,
                    PopupFadeDuration));
            if (ui_text_result_reward_value.gameObject.activeSelf)
            {
                resultTween.Insert(0.32f, DOTween.To(
                    () => ui_text_result_reward_value.alpha,
                    value => ui_text_result_reward_value.alpha = value,
                    1f,
                    0.18f));
            }
        }

        private void Configure(
            string title,
            string message,
            string reward,
            Sprite icon,
            string buttonLabel,
            Color buttonColor,
            Color panelColor,
            Color cardColor,
            bool danger)
        {
            ui_text_result_title_value.text = title;
            ui_text_result_message_value.text = message;
            ui_text_result_reward_value.text = reward;
            ui_text_result_reward_value.color = Color.white;
            ui_image_result_card_glow_value.sprite = ui_sprite_result_speed_lines;
            ui_text_result_reward_value.gameObject.SetActive(!string.IsNullOrWhiteSpace(reward));
            ui_image_result_icon_value.sprite = icon;
            ui_image_result_icon_value.preserveAspect = true;
            ui_text_result_primary_value.text = buttonLabel;
            ui_button_result_primary.image.color = buttonColor;
            ui_image_popup_panel_value.color = panelColor;

            Color cardSurfaceColor = Color.Lerp(
                new Color(0.03f, 0.008f, 0.008f, 1f),
                cardColor,
                0.38f);
            cardSurfaceColor.a = 1f;
            ui_image_result_card_value.color = cardSurfaceColor;
            ui_image_result_card_glow_value.color =
                new Color(cardColor.r, cardColor.g, cardColor.b, 0.22f);
            ui_image_result_card_border_value.color = cardColor;
            Outline cardOutline = ui_image_result_card_border_value.GetComponent<Outline>();
            cardOutline.effectColor =
                new Color(cardColor.r, cardColor.g, cardColor.b, 0.52f);

            ui_button_result_primary.gameObject.SetActive(!danger);
            ui_button_result_primary.interactable = !danger;
            ui_panel_bomb_actions.SetActive(danger);
            gameObject.SetActive(true);
            ui_canvas_group_result.alpha = 0f;
            ui_transform_popup_animator.localScale =
                danger ? new Vector3(0.82f, 0.82f, 1f) : Vector3.one;
            ui_transform_popup_animator.localRotation = Quaternion.identity;
            ui_image_result_icon_value.rectTransform.localScale =
                danger ? new Vector3(0.38f, 0.38f, 1f) : Vector3.one;
            ui_text_result_title_value.alpha = danger ? 0f : 1f;
            ui_text_result_message_value.alpha = danger ? 0f : 1f;
            ui_text_result_title_value.rectTransform.localScale =
                danger ? new Vector3(0.84f, 0.84f, 1f) : Vector3.one;
            ui_text_result_reward_value.rectTransform.localScale =
                danger ? new Vector3(0.84f, 0.84f, 1f) : Vector3.one;
            ui_text_result_reward_value.alpha = 0f;

            RectTransform primaryAnimator =
                (RectTransform)ui_button_result_primary.transform.parent;
            primaryAnimator.localScale = Vector3.one;
            ui_panel_bomb_actions.transform.localScale = Vector3.one;
            Overlay.color = danger
                ? new Color(0.28f, 0f, 0f, 0.90f)
                : new Color(0f, 0f, 0f, 0.82f);
        }

        private void ResetTransientState()
        {
            primaryAction = null;
            resultTween.Kill(false);
            resultCountTween.Kill(false);
            resultTween = null;
            resultCountTween = null;
            pendingRewardFlyTarget = null;
            pendingRewardFlySprite = null;
        }

        private void HandlePrimaryPressed()
        {
            CloseAndInvoke(primaryAction);
        }

    }
}
