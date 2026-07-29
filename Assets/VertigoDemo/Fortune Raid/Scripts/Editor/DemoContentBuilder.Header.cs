using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo.Editor
{
    public static partial class DemoContentBuilder
    {
        private static void CreateHeader(
            RectTransform parent,
            out TMP_Text zoneText,
            out TMP_Text zoneTypeText,
            out List<Image> zoneNodes,
            out List<TMP_Text> zoneLabels)
        {
            DemoUiFactory.Text(
                "ui_text_game_title", parent, "FORTUNE RAID", 34f, new Color(0.74f, 0.82f, 0.94f),
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(12f, -10f), new Vector2(360f, 50f), FontStyles.Bold).characterSpacing = 3f;
            zoneText = DemoUiFactory.Text(
                "ui_text_zone_value", parent, "ZONE 1", 42f, Color.white,
                TextAlignmentOptions.Center, new Vector2(0.38f, 1f), new Vector2(0.38f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -8f), new Vector2(360f, 54f), FontStyles.Bold);
            zoneTypeText = DemoUiFactory.Text(
                "ui_text_zone_type_value", parent, "RISK ZONE", 20f, new Color(1f, 0.46f, 0.2f),
                TextAlignmentOptions.Center, new Vector2(0.38f, 1f), new Vector2(0.38f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -62f), new Vector2(360f, 30f), FontStyles.Bold);

            RectTransform trail = DemoUiFactory.Rect(
                "ui_panel_zone_trail", parent, new Vector2(0.72f, 1f), new Vector2(0.72f, 1f),
                new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(620f, 70f));
            Image trailLine = DemoUiFactory.Image(
                "ui_image_zone_trail_line", trail, null, new Color(0.16f, 0.2f, 0.28f),
                new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0f, 5f));
            DemoUiFactory.Stretch(trailLine.rectTransform, 28f, -2.5f, 28f, -2.5f);

            zoneNodes = new List<Image>();
            zoneLabels = new List<TMP_Text>();
            for (int i = 0; i < 7; i++)
            {
                float x = -252f + i * 84f;
                Image node = DemoUiFactory.Image(
                    "ui_image_zone_node_" + i + "_value", trail, Sprite("ui_card_panel_zone_current_white.png"),
                    new Color(0.23f, 0.27f, 0.34f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f), new Vector2(x, 0f), new Vector2(62f, 62f));
                TMP_Text label = DemoUiFactory.Text(
                    "ui_text_zone_node_" + i + "_value", node.transform, (i + 1).ToString(), 19f, Color.white,
                    TextAlignmentOptions.Center, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                    Vector2.zero, Vector2.zero, FontStyles.Bold);
                zoneNodes.Add(node);
                zoneLabels.Add(label);
            }
        }

        private static void CreateBottomActions(RectTransform parent, out Button leaveButton)
        {
            leaveButton = DemoUiFactory.Button("ui_button_leave", parent, Sprite("UI_button_grey_standard.png"), new Color(0.24f, 0.53f, 0.72f), "COLLECT & LEAVE", new Vector2(0.875f, 0f), new Vector2(0f, 56f), new Vector2(330f, 90f));
        }

        private static void CreateSuperTransition(Transform parent, out GameObject panel, out CanvasGroup canvasGroup, out TMP_Text title, out TMP_Text punchline)
        {
            Image overlay = DemoUiFactory.Image("ui_panel_super_transition", parent, null, new Color(0.018f, 0.01f, 0.001f, 0.86f), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            DemoUiFactory.Stretch(overlay.rectTransform, 0f, 0f, 0f, 0f);
            panel = overlay.gameObject;
            canvasGroup = panel.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            title = DemoUiFactory.Text("ui_text_super_transition_title_value", overlay.transform, "GOLDEN ZONE 30", 54f, new Color(1f, 0.79f, 0.1f), TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 48f), new Vector2(900f, 82f), FontStyles.Bold);
            title.outlineColor = new Color32(88, 42, 0, 255);
            title.outlineWidth = 0.16f;
            title.characterSpacing = 2f;
            punchline = DemoUiFactory.Text(
                "ui_text_super_transition_punchline_value",
                overlay.transform,
                "SPECIAL REWARDS. NO BOMB.",
                25f,
                Color.white,
                TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -30f),
                new Vector2(900f, 48f),
                FontStyles.Bold);
            punchline.outlineColor = new Color32(50, 26, 0, 255);
            punchline.outlineWidth = 0.12f;
            punchline.characterSpacing = 3f;
        }
    }
}
