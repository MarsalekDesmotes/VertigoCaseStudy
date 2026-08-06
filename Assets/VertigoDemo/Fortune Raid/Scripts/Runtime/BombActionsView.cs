using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class BombActionsView : MonoBehaviour
    {
        private const float ButtonFadeDuration = 0.10f;

        [SerializeField] private Button ui_button_bomb_give_up;
        [SerializeField] private Button ui_button_bomb_currency_revive;
        [SerializeField] private Button ui_button_bomb_rewarded_revive;
        [SerializeField] private CanvasGroup ui_canvas_group_bomb_give_up;
        [SerializeField] private CanvasGroup ui_canvas_group_bomb_currency_revive;
        [SerializeField] private Image ui_image_bomb_revive_currency_value;
        [SerializeField] private TMP_Text ui_text_bomb_revive_cost_value;
        [SerializeField] private TMP_Text ui_text_bomb_give_up_label;
        [SerializeField] private TMP_Text ui_text_bomb_revive_label;

        private Action giveUpAction;
        private Action currencyReviveAction;

        private void Awake()
        {
            ui_button_bomb_give_up.onClick.AddListener(HandleGiveUpPressed);
            ui_button_bomb_currency_revive.onClick.AddListener(HandleCurrencyRevivePressed);
            ui_button_bomb_rewarded_revive.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            ui_button_bomb_give_up.onClick.RemoveListener(HandleGiveUpPressed);
            ui_button_bomb_currency_revive.onClick.RemoveListener(HandleCurrencyRevivePressed);
        }

        public void BindLabels(ILocalization localization)
        {
            ui_text_bomb_give_up_label.text = localization.Get(LocalizationKeys.PopupBombGiveUp);
            ui_text_bomb_revive_label.text = localization.Get(LocalizationKeys.PopupBombRevive);
        }

        public void Show(
            Sprite reviveCurrencyIcon,
            int reviveCost,
            bool canAffordRevive,
            Action onGiveUp,
            Action onCurrencyRevive,
            Sequence openSequence,
            float firstRevealTime,
            float revealStagger,
            Action<Action> closeAndInvoke)
        {
            giveUpAction = () => closeAndInvoke(onGiveUp);
            currencyReviveAction = () => closeAndInvoke(onCurrencyRevive);

            ui_image_bomb_revive_currency_value.sprite = reviveCurrencyIcon;
            ui_image_bomb_revive_currency_value.gameObject.SetActive(true);
            ui_text_bomb_revive_cost_value.text = reviveCost.ToString();
            ui_button_bomb_rewarded_revive.gameObject.SetActive(false);
            gameObject.SetActive(true);

            PrepareButton(ui_button_bomb_give_up, ui_canvas_group_bomb_give_up);
            PrepareButton(ui_button_bomb_currency_revive, ui_canvas_group_bomb_currency_revive);

            InsertButtonReveal(
                openSequence,
                ui_button_bomb_give_up,
                ui_canvas_group_bomb_give_up,
                firstRevealTime,
                true);
            InsertButtonReveal(
                openSequence,
                ui_button_bomb_currency_revive,
                ui_canvas_group_bomb_currency_revive,
                firstRevealTime + revealStagger,
                canAffordRevive);
        }

        public void Hide()
        {
            giveUpAction = null;
            currencyReviveAction = null;
            gameObject.SetActive(false);
        }

        private void HandleGiveUpPressed()
        {
            Action action = giveUpAction;
            giveUpAction = null;
            currencyReviveAction = null;
            action?.Invoke();
        }

        private void HandleCurrencyRevivePressed()
        {
            if (!ui_button_bomb_currency_revive.interactable)
            {
                return;
            }

            Action action = currencyReviveAction;
            giveUpAction = null;
            currencyReviveAction = null;
            action?.Invoke();
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
            bool canInteract)
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
                button.interactable = canInteract;
                canvasGroup.interactable = canInteract;
                canvasGroup.blocksRaycasts = canInteract;
            });
        }
    }
}
