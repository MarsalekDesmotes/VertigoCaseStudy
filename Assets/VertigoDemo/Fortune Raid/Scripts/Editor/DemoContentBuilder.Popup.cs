using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.Editor
{
    public static partial class DemoContentBuilder
    {
        private static void CreateResultPopup(Transform parent, out GameObject popup, out RectTransform animator, out CanvasGroup canvasGroup, out Image panel, out Image card, out Image cardGlow, out Image cardBorder, out TMP_Text title, out TMP_Text message, out TMP_Text reward, out Image icon, out Button primaryButton, out TMP_Text primaryButtonText, out GameObject bombActions, out Button giveUpButton, out Button currencyReviveButton, out Button rewardedReviveButton, out Image reviveCurrencyIcon, out TMP_Text reviveCostText)
        {
            Image overlay = DemoUiFactory.Image("ui_popup_result", parent, null, new Color(0f, 0f, 0f, 0.82f), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, raycast: true);
            DemoUiFactory.Stretch(overlay.rectTransform, 0f, 0f, 0f, 0f);
            popup = overlay.gameObject;
            canvasGroup = popup.AddComponent<CanvasGroup>();
            animator = DemoUiFactory.Rect("ui_transform_popup_animator", overlay.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(860f, 800f));
            panel = DemoUiFactory.Image("ui_image_popup_surface_value", animator, null, new Color(0.08f, 0.02f, 0.025f, 0.99f), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-18f, -18f));
            DemoUiFactory.Image("ui_image_popup_border", animator, Sprite("ui_card_frame_12px_neutral.png"), new Color(1f, 0.56f, 0.08f, 0.48f), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            title = DemoUiFactory.Text("ui_text_result_title_value", animator, "OH NO, THE BOMB EXPLODED!", 36f, Color.white, TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -38f), new Vector2(-54f, 68f), FontStyles.Bold);
            message = DemoUiFactory.Text("ui_text_result_message_value", animator, "All rewards from this run were lost.", 21f, new Color(0.82f, 0.85f, 0.9f), TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -108f), new Vector2(-80f, 52f), FontStyles.Normal);
            card = DemoUiFactory.Image("ui_image_result_card_value", animator, Sprite("ui_card_panel_zone_bg.png"), new Color(0.36f, 0.025f, 0.018f, 1f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(292f, 390f));
            RectTransform cardContent = DemoUiFactory.Rect("ui_panel_result_card_content", animator, new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(278f, 376f));
            cardContent.gameObject.AddComponent<RectMask2D>();
            cardGlow = DemoUiFactory.Image("ui_image_result_card_glow_value", cardContent, Sprite("star_flash_alpha.png"), new Color(0.92f, 0.04f, 0.02f, 0.34f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(376f, 376f), raycast: false, preserveAspect: true);
            ((MaskableGraphic)cardGlow).maskable = true;
            PopupSunrayView cardSunrayView = cardGlow.gameObject.AddComponent<PopupSunrayView>();
            SerializedObject cardSunraySo = new SerializedObject(cardSunrayView);
            Set(cardSunraySo, "ui_transform_sunray_animator", cardGlow.rectTransform);
            Set(cardSunraySo, "ui_image_sunray_value", cardGlow);
            cardSunraySo.FindProperty("rotationSpeedDegrees").floatValue = 8f;
            cardSunraySo.FindProperty("minimumAlpha").floatValue = 0.28f;
            cardSunraySo.FindProperty("maximumAlpha").floatValue = 0.46f;
            cardSunraySo.ApplyModifiedPropertiesWithoutUndo();
            icon = DemoUiFactory.Image("ui_image_result_icon_value", cardContent, Sprite("ui_card_icon_death.png"), Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(250f, 250f), raycast: false, preserveAspect: true);
            ((MaskableGraphic)icon).maskable = true;
            cardBorder = DemoUiFactory.Image("ui_image_result_card_border_value", animator, Sprite("ui_card_frame_4px_zone.png"), new Color(0.92f, 0.07f, 0.035f, 1f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(306f, 406f));
            Outline cardOutline = cardBorder.gameObject.AddComponent<Outline>();
            ((Shadow)cardOutline).effectColor = new Color(1f, 0.02f, 0.01f, 0.52f);
            ((Shadow)cardOutline).effectDistance = new Vector2(3f, -3f);
            ((Shadow)cardOutline).useGraphicAlpha = true;
            reward = DemoUiFactory.Text("ui_text_result_reward_value", animator, "Pistol Points  x40", 30f, Color.white, TextAlignmentOptions.Center, new Vector2(0.5f, 0.255f), new Vector2(0.5f, 0.255f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560f, 62f), FontStyles.Bold);
            primaryButton = DemoUiFactory.Button("ui_button_result_primary", animator, Sprite("UI_button_grey_standard.png"), new Color(0.72f, 0.12f, 0.08f), "RESTART", new Vector2(0.5f, 0f), new Vector2(0f, 56f), new Vector2(350f, 96f));
            primaryButtonText = primaryButton.GetComponentInChildren<TMP_Text>(true);
            RectTransform bombActionsRoot = DemoUiFactory.Rect("ui_panel_bomb_actions", animator, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 54f), new Vector2(700f, 100f));
            bombActions = bombActionsRoot.gameObject;
            giveUpButton = DemoUiFactory.Button("ui_button_bomb_give_up", bombActionsRoot, Sprite("UI_button_grey_standard.png"), new Color(0.72f, 0.74f, 0.78f), "GIVE UP", new Vector2(0.16f, 0.5f), Vector2.zero, new Vector2(200f, 82f));
            TMP_Text giveUpLabel = giveUpButton.GetComponentInChildren<TMP_Text>(true);
            giveUpLabel.fontSize = 21f;
            currencyReviveButton = DemoUiFactory.Button("ui_button_bomb_currency_revive", bombActionsRoot, Sprite("UI_button_grey_standard.png"), new Color(0.18f, 0.78f, 0.1f), "REVIVE", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(200f, 82f));
            TMP_Text currencyReviveLabel = currencyReviveButton.GetComponentInChildren<TMP_Text>(true);
            currencyReviveLabel.fontSize = 21f;
            currencyReviveLabel.rectTransform.anchorMin = new Vector2(0f, 0.34f);
            currencyReviveLabel.rectTransform.anchorMax = Vector2.one;
            currencyReviveLabel.rectTransform.offsetMin = Vector2.zero;
            currencyReviveLabel.rectTransform.offsetMax = Vector2.zero;
            reviveCurrencyIcon = DemoUiFactory.Image("ui_image_bomb_revive_currency_value", currencyReviveButton.transform, Sprite("UI_icon_gold.png"), Color.white, new Vector2(0.38f, 0.08f), new Vector2(0.5f, 0.34f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, raycast: false, preserveAspect: true);
            reviveCostText = DemoUiFactory.Text("ui_text_bomb_revive_cost_value", currencyReviveButton.transform, "25", 15f, Color.white, TextAlignmentOptions.Left, new Vector2(0.5f, 0.08f), new Vector2(0.68f, 0.34f), new Vector2(0f, 0.5f), Vector2.zero, Vector2.zero, FontStyles.Bold);
            rewardedReviveButton = DemoUiFactory.Button("ui_button_bomb_rewarded_revive", bombActionsRoot, Sprite("UI_button_grey_standard.png"), new Color(0.16f, 0.46f, 0.95f), "REVIVE", new Vector2(0.84f, 0.5f), Vector2.zero, new Vector2(200f, 82f));
            TMP_Text rewardedReviveLabel = rewardedReviveButton.GetComponentInChildren<TMP_Text>(true);
            rewardedReviveLabel.fontSize = 21f;
            rewardedReviveLabel.rectTransform.anchorMin = new Vector2(0f, 0.34f);
            rewardedReviveLabel.rectTransform.anchorMax = Vector2.one;
            rewardedReviveLabel.rectTransform.offsetMin = Vector2.zero;
            rewardedReviveLabel.rectTransform.offsetMax = Vector2.zero;
            DemoUiFactory.Text("ui_text_bomb_rewarded_hint_value", rewardedReviveButton.transform, "WATCH AD", 13f, Color.white, TextAlignmentOptions.Center, new Vector2(0f, 0.06f), new Vector2(1f, 0.34f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, FontStyles.Bold);
            bombActions.SetActive(false);
        }
    }
}

