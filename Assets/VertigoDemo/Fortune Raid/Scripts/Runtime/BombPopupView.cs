using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class BombPopupView : MonoBehaviour
    {
        private const float FirstButtonRevealTime = 0.42f;
        private const float ButtonRevealStagger = 0.06f;
        private const float ButtonFadeDuration = 0.10f;
        private const float ImpactFlashDuration = 0.10f;

        [SerializeField] private ResultPopupView ui_result_popup_view;
        [SerializeField] private WheelView ui_wheel_view;
        [SerializeField] private Button ui_button_bomb_give_up;
        [SerializeField] private Button ui_button_bomb_currency_revive;
        [SerializeField] private Button ui_button_bomb_rewarded_revive;
        [SerializeField] private CanvasGroup ui_canvas_group_bomb_give_up;
        [SerializeField] private CanvasGroup ui_canvas_group_bomb_currency_revive;
        [SerializeField] private CanvasGroup ui_canvas_group_bomb_rewarded_revive;
        [SerializeField] private Image ui_image_bomb_revive_currency_value;
        [SerializeField] private TMP_Text ui_text_bomb_revive_cost_value;
        [SerializeField] private Image ui_image_bomb_impact_flash_value;

        private Action giveUpAction;
        private Action currencyReviveAction;
        private Action rewardedReviveAction;
        private Tween impactFlashTween;
        private bool currencyReviveAvailable;

        private void Awake()
        {
            ui_button_bomb_give_up.onClick.AddListener(HandleGiveUpPressed);
            ui_button_bomb_currency_revive.onClick.AddListener(HandleCurrencyRevivePressed);
            ui_button_bomb_rewarded_revive.onClick.AddListener(HandleRewardedRevivePressed);
        }

        private void OnDestroy()
        {
            impactFlashTween.Kill(false);
            ui_button_bomb_give_up.onClick.RemoveListener(HandleGiveUpPressed);
            ui_button_bomb_currency_revive.onClick.RemoveListener(HandleCurrencyRevivePressed);
            ui_button_bomb_rewarded_revive.onClick.RemoveListener(HandleRewardedRevivePressed);
        }

        public void Show(
            Sprite bombIcon,
            Sprite reviveCurrencyIcon,
            int reviveCost,
            bool canAffordRevive,
            Action onGiveUp,
            Action onCurrencyRevive,
            Action onRewardedRevive)
        {
            giveUpAction = onGiveUp;
            currencyReviveAction = onCurrencyRevive;
            rewardedReviveAction = onRewardedRevive;
            currencyReviveAvailable = canAffordRevive;
            ui_image_bomb_revive_currency_value.sprite = reviveCurrencyIcon;
            ui_image_bomb_revive_currency_value.gameObject.SetActive(true);
            ui_text_bomb_revive_cost_value.text = Mathf.Max(0, reviveCost).ToString();

            PrepareButton(ui_button_bomb_give_up, ui_canvas_group_bomb_give_up);
            PrepareButton(
                ui_button_bomb_currency_revive,
                ui_canvas_group_bomb_currency_revive);
            PrepareButton(
                ui_button_bomb_rewarded_revive,
                ui_canvas_group_bomb_rewarded_revive);

            Sequence sequence = ui_result_popup_view.ShowBombFrame(bombIcon);
            InsertButtonReveal(
                sequence,
                ui_button_bomb_give_up,
                ui_canvas_group_bomb_give_up,
                FirstButtonRevealTime,
                () => true);
            InsertButtonReveal(
                sequence,
                ui_button_bomb_currency_revive,
                ui_canvas_group_bomb_currency_revive,
                FirstButtonRevealTime + ButtonRevealStagger,
                () => currencyReviveAvailable);
            InsertButtonReveal(
                sequence,
                ui_button_bomb_rewarded_revive,
                ui_canvas_group_bomb_rewarded_revive,
                FirstButtonRevealTime + ButtonRevealStagger * 2f,
                () => true);
        }

        public void PlayImpact()
        {
            ui_wheel_view.PlayBombImpact();
            impactFlashTween.Kill(false);
            ui_image_bomb_impact_flash_value.gameObject.SetActive(true);
            ui_image_bomb_impact_flash_value.color =
                new Color(0.92f, 0.015f, 0.01f, 0.78f);
            impactFlashTween = DOTween.To(
                    () => ui_image_bomb_impact_flash_value.color.a,
                    value =>
                    {
                        Color color = ui_image_bomb_impact_flash_value.color;
                        color.a = value;
                        ui_image_bomb_impact_flash_value.color = color;
                    },
                    0f,
                    ImpactFlashDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .SetTarget(ui_image_bomb_impact_flash_value)
                .OnComplete(() =>
                {
                    ui_image_bomb_impact_flash_value.gameObject.SetActive(false);
                    impactFlashTween = null;
                });
        }

        public void ResetState()
        {
            giveUpAction = null;
            currencyReviveAction = null;
            rewardedReviveAction = null;
            impactFlashTween.Kill(false);
            impactFlashTween = null;
            ui_image_bomb_impact_flash_value.gameObject.SetActive(false);
        }

        private static void PrepareButton(Button button, CanvasGroup canvasGroup)
        {
            button.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
            button.interactable = false;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private static void InsertButtonReveal(
            Sequence sequence,
            Button button,
            CanvasGroup canvasGroup,
            float startTime,
            Func<bool> canInteract)
        {
            sequence.Insert(startTime, button.transform
                .DOScale(Vector3.one, 0.14f)
                .SetEase(Ease.OutBack));
            sequence.Insert(startTime, DOTween.To(
                    () => canvasGroup.alpha,
                    value => canvasGroup.alpha = value,
                    1f,
                    ButtonFadeDuration)
                .SetEase(Ease.OutQuad));
            sequence.InsertCallback(startTime, () =>
            {
                bool interactable = canInteract();
                button.interactable = interactable;
                canvasGroup.interactable = interactable;
                canvasGroup.blocksRaycasts = interactable;
            });
        }

        private void HandleGiveUpPressed()
        {
            Action action = giveUpAction;
            ResetState();
            ui_result_popup_view.CloseAndInvoke(action);
        }

        private void HandleCurrencyRevivePressed()
        {
            if (!ui_button_bomb_currency_revive.interactable)
            {
                return;
            }

            Action action = currencyReviveAction;
            ResetState();
            ui_result_popup_view.CloseAndInvoke(action);
        }

        private void HandleRewardedRevivePressed()
        {
            Action action = rewardedReviveAction;
            ResetState();
            ui_result_popup_view.CloseAndInvoke(action);
        }

    }
}
