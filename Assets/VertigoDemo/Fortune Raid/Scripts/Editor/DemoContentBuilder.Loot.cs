using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.Editor
{
    public static partial class DemoContentBuilder
    {
        private static void CreateRewardPanel(
            RectTransform parent,
            int rowCapacity,
            out List<RewardRowView> rows,
            out GameObject emptyState,
            out ScrollRect scrollRect,
            out RectTransform scrollContent)
        {
            Image panel = DemoUiFactory.Image("ui_panel_collected_rewards", parent, Sprite("ui_card_frame_12px_neutral.png"), new Color(0.055f, 0.075f, 0.12f, 0.98f), new Vector2(0.76f, 0.2f), new Vector2(0.99f, 0.8f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            DemoUiFactory.Stretch(panel.rectTransform, 0f, 0f, 0f, 0f);
            DemoUiFactory.Text("ui_text_collected_title", panel.transform, "RUN LOOT", 28f, new Color(1f, 0.78f, 0.14f), TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(-24f, 46f), FontStyles.Bold);
            DemoUiFactory.Text("ui_text_collected_subtitle", panel.transform, "Secure it at a safe zone", 16f, new Color(0.55f, 0.62f, 0.72f), TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -64f), new Vector2(-24f, 30f), FontStyles.Normal);
            Image viewportImage = DemoUiFactory.Image("ui_viewport_loot", panel.transform, null, Color.clear, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(0f, -45f), new Vector2(-32f, -122f), raycast: true);
            viewportImage.gameObject.AddComponent<RectMask2D>();
            scrollContent = DemoUiFactory.Rect("ui_transform_loot_scroll_content", viewportImage.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 422f));
            scrollRect = panel.gameObject.AddComponent<ScrollRect>();
            scrollRect.viewport = viewportImage.rectTransform;
            scrollRect.content = scrollContent;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.12f;
            scrollRect.scrollSensitivity = 28f;
            RectTransform emptyRoot = DemoUiFactory.Rect("ui_panel_loot_empty_state", panel.transform, new Vector2(0.08f, 0.16f), new Vector2(0.92f, 0.74f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            emptyState = emptyRoot.gameObject;
            DemoUiFactory.Text("ui_text_loot_empty_title", emptyRoot, "NO REWARDS YET", 19f, new Color(0.72f, 0.78f, 0.88f), TextAlignmentOptions.Center, new Vector2(0f, 0.48f), new Vector2(1f, 0.62f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, FontStyles.Bold);
            DemoUiFactory.Text("ui_text_loot_empty_hint", emptyRoot, "Spin the wheel to begin", 14f, new Color(0.42f, 0.52f, 0.66f), TextAlignmentOptions.Center, new Vector2(0f, 0.36f), new Vector2(1f, 0.48f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, FontStyles.Normal);
            rows = new List<RewardRowView>();
            for (int i = 0; i < rowCapacity; i++)
            {
                Image rowBg = DemoUiFactory.Image("ui_panel_reward_row_" + i, scrollContent, Sprite("ui_card_frame_4px_zone.png"), new Color(0.13f, 0.17f, 0.24f, 0.95f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -i * 72f), new Vector2(0f, 62f));
                ((MaskableGraphic)rowBg).maskable = true;
                RectTransform rowRoot = rowBg.rectTransform;
                RewardRowView row = rowRoot.gameObject.AddComponent<RewardRowView>();
                Image icon = DemoUiFactory.Image("ui_image_collected_reward_value", rowRoot, Sprite("UI_icon_gold.png"), Color.white, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(72f, 52f), raycast: false, preserveAspect: true);
                ((MaskableGraphic)icon).maskable = true;
                TMP_Text rewardName = DemoUiFactory.Text("ui_text_collected_reward_name_value", rowRoot, "Gold", 18f, new Color(0.84f, 0.88f, 0.94f), TextAlignmentOptions.Left, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(-8f, 0f), new Vector2(-210f, 0f), FontStyles.Bold);
                rewardName.enableAutoSizing = true;
                rewardName.fontSizeMin = 13f;
                rewardName.fontSizeMax = 18f;
                rewardName.enableWordWrapping = false;
                ((MaskableGraphic)rewardName).maskable = true;
                TMP_Text amount = DemoUiFactory.Text("ui_text_collected_amount_value", rowRoot, "x0", 23f, Color.white, TextAlignmentOptions.Right, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-16f, 0f), new Vector2(92f, 48f), FontStyles.Bold);
                ((MaskableGraphic)amount).maskable = true;
                SerializedObject rowSo = new SerializedObject(row);
                Set(rowSo, "ui_image_collected_row_background_value", rowBg);
                Set(rowSo, "ui_image_collected_reward_value", icon);
                Set(rowSo, "ui_text_collected_reward_name_value", rewardName);
                Set(rowSo, "ui_text_collected_amount_value", amount);
                rowSo.ApplyModifiedPropertiesWithoutUndo();
                rowRoot.gameObject.SetActive(false);
                rows.Add(row);
            }
        }
    }
}
