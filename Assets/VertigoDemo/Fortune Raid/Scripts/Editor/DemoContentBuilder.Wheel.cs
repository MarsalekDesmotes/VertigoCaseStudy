using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.Editor
{
    public static partial class DemoContentBuilder
    {
        private static WheelView CreateWheel(RectTransform parent, out Button spinButton)
        {
            RectTransform wheelFrame = DemoUiFactory.Rect("ui_panel_wheel_frame", parent, new Vector2(0.12f, 0f), new Vector2(0.82f, 0.92f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            RectTransform wheelRoot = DemoUiFactory.Rect("ui_panel_wheel", wheelFrame, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            AspectRatioFitter wheelAspect = wheelRoot.gameObject.AddComponent<AspectRatioFitter>();
            wheelAspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            wheelAspect.aspectRatio = 1f;
            WheelView wheelView = wheelRoot.gameObject.AddComponent<WheelView>();
            RectTransform animator = DemoUiFactory.Rect("ui_transform_wheel_animator", wheelRoot, new Vector2(0.045f, 0.045f), new Vector2(0.955f, 0.955f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            Image wheelBase = DemoUiFactory.Image("ui_image_wheel_base_value", animator, Sprite("ui_spin_bronze_base.png"), Color.white, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, raycast: false, preserveAspect: true);
            List<WheelSliceView> slices = new List<WheelSliceView>();
            for (int i = 0; i < 8; i++)
            {
                float angle = 90f - i * 45f;
                Vector2 normalizedPosition = new Vector2(0.5f, 0.5f) +
                    new Vector2(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        Mathf.Sin(angle * Mathf.Deg2Rad)) * 0.296f;
                Vector2 halfSize = new Vector2(0.102f, 0.102f);
                RectTransform sliceRoot = DemoUiFactory.Rect("ui_panel_wheel_slice_" + i, animator, normalizedPosition - halfSize, normalizedPosition + halfSize, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
                CanvasGroup sliceCanvasGroup = sliceRoot.gameObject.AddComponent<CanvasGroup>();
                WheelSliceView sliceView = sliceRoot.gameObject.AddComponent<WheelSliceView>();
                RectTransform rewardAnimator = DemoUiFactory.Rect("ui_transform_reward_animator", sliceRoot, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
                Image rewardGlow = DemoUiFactory.Image("ui_image_reward_glow_value", rewardAnimator, Sprite("star_glow_alpha.png"), new Color(1f, 0.62f, 0.08f, 0f), new Vector2(0.02f, 0.1f), new Vector2(0.98f, 0.98f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, raycast: false, preserveAspect: true);
                Image specialShine = DemoUiFactory.Image("ui_image_special_shine_value", rewardAnimator, Sprite("ui_vfx_offer_shine.tga"), new Color(1f, 0.72f, 0.16f, 0.26f), new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.98f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, raycast: false, preserveAspect: true);
                specialShine.gameObject.SetActive(false);
                Image specialSparkle = DemoUiFactory.Image("ui_image_special_sparkle_value", rewardAnimator, Sprite("star_glow_alpha.png"), new Color(1f, 0.92f, 0.54f, 0.84f), new Vector2(0.78f, 0.8f), new Vector2(0.78f, 0.8f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(48f, 48f), raycast: false, preserveAspect: true);
                specialSparkle.gameObject.SetActive(false);
                Image icon = DemoUiFactory.Image("ui_image_reward_value", rewardAnimator, Sprite("UI_icon_gold.png"), Color.white, new Vector2(0.16f, 0.31f), new Vector2(0.84f, 0.89f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, raycast: false, preserveAspect: true);
                TMP_Text amount = DemoUiFactory.Text("ui_text_amount_value", rewardAnimator, "x1", 22f, Color.white, TextAlignmentOptions.Center, new Vector2(0.1f, 0.02f), new Vector2(0.9f, 0.3f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, FontStyles.Bold);
                Shadow shadow = amount.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
                shadow.effectDistance = new Vector2(2f, -2f);
                SerializedObject sliceSo = new SerializedObject(sliceView);
                Set(sliceSo, "ui_canvas_group_slice", sliceCanvasGroup);
                Set(sliceSo, "ui_transform_reward_animator", rewardAnimator);
                Set(sliceSo, "ui_image_reward_glow_value", rewardGlow);
                Set(sliceSo, "ui_image_special_shine_value", specialShine);
                Set(sliceSo, "ui_image_special_sparkle_value", specialSparkle);
                Set(sliceSo, "ui_image_reward_value", icon);
                Set(sliceSo, "ui_text_amount_value", amount);
                sliceSo.ApplyModifiedPropertiesWithoutUndo();
                slices.Add(sliceView);
            }
            spinButton = DemoUiFactory.Button("ui_button_spin", wheelRoot, Sprite("ui_spin_generic_button.png"), Color.white, "SPIN", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(236f, 236f));
            Image center = ((Selectable)spinButton).image;
            Shadow centerShadow = center.gameObject.AddComponent<Shadow>();
            centerShadow.effectColor = new Color(0f, 0f, 0f, 0.75f);
            centerShadow.effectDistance = new Vector2(0f, -5f);
            TMP_Text spinLabel = spinButton.GetComponentInChildren<TMP_Text>(true);
            spinLabel.fontSize = 42f;
            spinLabel.fontStyle = FontStyles.Bold;
            spinLabel.alignment = TextAlignmentOptions.Center;
            Image indicator = DemoUiFactory.Image("ui_image_wheel_indicator_value", wheelRoot, Sprite("ui_spin_bronze_indicator.png"), Color.white, new Vector2(0.445f, 0.87f), new Vector2(0.555f, 1.025f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, raycast: false, preserveAspect: true);
            SerializedObject wheelSo = new SerializedObject(wheelView);
            Set(wheelSo, "ui_transform_wheel_animator", animator);
            Set(wheelSo, "ui_image_wheel_base_value", wheelBase);
            Set(wheelSo, "ui_image_wheel_indicator_value", indicator);
            SetList<WheelSliceView>(wheelSo, "ui_wheel_slices", slices);
            wheelSo.ApplyModifiedPropertiesWithoutUndo();
            return wheelView;
        }
    }
}

