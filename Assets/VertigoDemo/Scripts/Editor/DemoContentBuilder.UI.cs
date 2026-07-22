using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VertigoDemo.Editor
{
    public static partial class DemoContentBuilder
    {
        private static GameObject BuildGamePrefab(WheelCatalog catalog)
        {
            GameObject root = new GameObject(
                "ui_screen_game",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(GameScreenView),
                typeof(GameController),
                typeof(AutomatedDemoDriver),
                typeof(AutomatedVideoFrameCapture),
                typeof(AutomatedScreenshotCapture));
            root.GetComponent<AutomatedDemoDriver>().Configure(
                root.GetComponent<GameController>(), root.GetComponent<GameScreenView>());
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            root.GetComponent<GraphicRaycaster>().blockingObjects = GraphicRaycaster.BlockingObjects.None;

            Image background = DemoUiFactory.Image(
                "ui_image_background", root.transform, null, new Color(0.018f, 0.025f, 0.045f),
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            DemoUiFactory.Stretch(background.rectTransform, 0f, 0f, 0f, 0f);

            Image topGlow = DemoUiFactory.Image(
                "ui_image_top_glow", root.transform, Sprite("star_flash_alpha.png"), new Color(0.12f, 0.34f, 0.68f, 0.10f),
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, 170f), new Vector2(1000f, 1000f), false, true);
            topGlow.transform.localRotation = Quaternion.Euler(0f, 0f, 12f);

            RectTransform safeArea = DemoUiFactory.Rect(
                "ui_panel_safe_area", root.transform, Vector2.zero, Vector2.one,
                new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            DemoUiFactory.Stretch(safeArea, 42f, 34f, 42f, 28f);

            CreateHeader(safeArea, out TMP_Text zoneText, out TMP_Text zoneTypeText,
                out List<Image> zoneNodes, out List<TMP_Text> zoneLabels);
            WheelView wheelView = CreateWheel(safeArea);
            CreateRewardPanel(safeArea, out List<RewardRowView> rewardRows);
            CreateBottomActions(safeArea, out Button spinButton, out Button leaveButton, out TMP_Text leaveHint);
            CreateResultPopup(root.transform, out GameObject popup, out RectTransform popupAnimator,
                out TMP_Text resultTitle, out TMP_Text resultMessage, out Image resultIcon,
                out Button resultButton, out TMP_Text resultButtonText);

            GameScreenView screen = root.GetComponent<GameScreenView>();
            SerializedObject screenSo = new SerializedObject(screen);
            Set(screenSo, "ui_wheel_view", wheelView);
            Set(screenSo, "ui_text_zone_value", zoneText);
            Set(screenSo, "ui_text_zone_type_value", zoneTypeText);
            Set(screenSo, "ui_button_spin", spinButton);
            Set(screenSo, "ui_button_leave", leaveButton);
            Set(screenSo, "ui_text_leave_hint_value", leaveHint);
            SetList(screenSo, "ui_reward_rows", rewardRows);
            SetList(screenSo, "ui_zone_nodes", zoneNodes);
            SetList(screenSo, "ui_zone_node_labels_value", zoneLabels);
            Set(screenSo, "ui_popup_result", popup);
            Set(screenSo, "ui_transform_popup_animator", popupAnimator);
            Set(screenSo, "ui_text_result_title_value", resultTitle);
            Set(screenSo, "ui_text_result_message_value", resultMessage);
            Set(screenSo, "ui_image_result_icon_value", resultIcon);
            Set(screenSo, "ui_button_result_primary", resultButton);
            Set(screenSo, "ui_text_result_primary_value", resultButtonText);
            screenSo.ApplyModifiedPropertiesWithoutUndo();

            GameController controller = root.GetComponent<GameController>();
            SerializedObject controllerSo = new SerializedObject(controller);
            Set(controllerSo, "wheelCatalog", catalog);
            Set(controllerSo, "gameScreenView", screen);
            Set(controllerSo, "bombIcon", Sprite("ui_card_icon_death.png"));
            Set(controllerSo, "collectedChestIcon", Sprite("UI_icon_chest_gold_nolight.png"));
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            GameObject eventSystem = new GameObject("ui_system_events", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(root.transform, false);
            popup.SetActive(false);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, GamePrefabPath);
            Object.DestroyImmediate(root);
            return prefab;
        }

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
                new Vector2(12f, -10f), new Vector2(360f, 50f), FontStyles.Bold);
            DemoUiFactory.Text(
                "ui_text_game_subtitle", parent, "SPIN. RISK. EXTRACT.", 17f, new Color(0.32f, 0.54f, 0.78f),
                TextAlignmentOptions.Left, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(14f, -58f), new Vector2(360f, 30f), FontStyles.Bold);

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
                new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(650f, 70f));
            Image trailLine = DemoUiFactory.Image(
                "ui_image_zone_trail_line", trail, null, new Color(0.16f, 0.2f, 0.28f),
                new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0f, 5f));
            DemoUiFactory.Stretch(trailLine.rectTransform, 28f, -2.5f, 28f, -2.5f);

            zoneNodes = new List<Image>();
            zoneLabels = new List<TMP_Text>();
            for (int i = 0; i < 7; i++)
            {
                float x = -270f + i * 90f;
                Image node = DemoUiFactory.Image(
                    "ui_image_zone_node_" + i + "_value", trail, Sprite("ui_card_panel_zone_current_white.png"),
                    new Color(0.23f, 0.27f, 0.34f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f), new Vector2(x, 0f), new Vector2(56f, 56f));
                TMP_Text label = DemoUiFactory.Text(
                    "ui_text_zone_node_" + i + "_value", node.transform, (i + 1).ToString(), 19f, Color.white,
                    TextAlignmentOptions.Center, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                    Vector2.zero, Vector2.zero, FontStyles.Bold);
                zoneNodes.Add(node);
                zoneLabels.Add(label);
            }
        }

        private static WheelView CreateWheel(RectTransform parent)
        {
            RectTransform wheelRoot = DemoUiFactory.Rect(
                "ui_panel_wheel", parent, new Vector2(0.38f, 0.52f), new Vector2(0.38f, 0.52f),
                new Vector2(0.5f, 0.5f), new Vector2(0f, -5f), new Vector2(570f, 620f));
            WheelView wheelView = wheelRoot.gameObject.AddComponent<WheelView>();

            RectTransform animator = DemoUiFactory.Rect(
                "ui_transform_wheel_animator", wheelRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(540f, 540f));
            Image wheelBase = DemoUiFactory.Image(
                "ui_image_wheel_base_value", animator, Sprite("ui_spin_bronze_base.png"), Color.white,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(540f, 540f), false, true);

            List<WheelSliceView> slices = new List<WheelSliceView>();
            for (int i = 0; i < 8; i++)
            {
                float angle = 90f - i * 45f;
                Vector2 position = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 204f;
                RectTransform sliceRoot = DemoUiFactory.Rect(
                    "ui_panel_wheel_slice_" + i, animator, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f), position, new Vector2(105f, 105f));
                WheelSliceView sliceView = sliceRoot.gameObject.AddComponent<WheelSliceView>();
                Image icon = DemoUiFactory.Image(
                    "ui_image_reward_value", sliceRoot, Sprite("UI_icon_gold.png"), Color.white,
                    new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(62f, 56f), false, true);
                TMP_Text amount = DemoUiFactory.Text(
                    "ui_text_amount_value", sliceRoot, "x1", 18f, Color.white,
                    TextAlignmentOptions.Center, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                    new Vector2(0f, 2f), new Vector2(94f, 28f), FontStyles.Bold);
                Shadow shadow = amount.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
                shadow.effectDistance = new Vector2(1.5f, -1.5f);

                SerializedObject sliceSo = new SerializedObject(sliceView);
                Set(sliceSo, "ui_image_reward_value", icon);
                Set(sliceSo, "ui_text_amount_value", amount);
                sliceSo.ApplyModifiedPropertiesWithoutUndo();
                slices.Add(sliceView);
            }

            Image center = DemoUiFactory.Image(
                "ui_image_wheel_center", wheelRoot, Sprite("ui_spin_generic_button.png"), Color.white,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, -10f), new Vector2(158f, 158f), false, true);
            center.gameObject.AddComponent<Shadow>().effectColor = new Color(0f, 0f, 0f, 0.75f);
            DemoUiFactory.Text(
                "ui_text_wheel_center", center.transform, "LUCKY", 17f, new Color(1f, 0.78f, 0.14f),
                TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 27f), new Vector2(110f, 24f), FontStyles.Bold);

            Image indicator = DemoUiFactory.Image(
                "ui_image_wheel_indicator_value", wheelRoot, Sprite("ui_spin_bronze_indicator.png"), Color.white,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, 12f), new Vector2(62f, 84f), false, true);

            SerializedObject wheelSo = new SerializedObject(wheelView);
            Set(wheelSo, "ui_transform_wheel_animator", animator);
            Set(wheelSo, "ui_image_wheel_base_value", wheelBase);
            Set(wheelSo, "ui_image_wheel_indicator_value", indicator);
            SetList(wheelSo, "ui_wheel_slices", slices);
            wheelSo.ApplyModifiedPropertiesWithoutUndo();
            return wheelView;
        }

        private static void CreateRewardPanel(RectTransform parent, out List<RewardRowView> rows)
        {
            Image panel = DemoUiFactory.Image(
                "ui_panel_collected_rewards", parent, Sprite("ui_card_frame_12px_neutral.png"),
                new Color(0.08f, 0.11f, 0.17f, 0.98f), new Vector2(0.79f, 0.17f), new Vector2(0.98f, 0.82f),
                new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            DemoUiFactory.Stretch(panel.rectTransform, 0f, 0f, 0f, 0f);
            DemoUiFactory.Text(
                "ui_text_collected_title", panel.transform, "RUN LOOT", 28f, new Color(1f, 0.78f, 0.14f),
                TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -20f), new Vector2(-24f, 46f), FontStyles.Bold);
            DemoUiFactory.Text(
                "ui_text_collected_subtitle", panel.transform, "Secure it at a safe zone", 16f, new Color(0.55f, 0.62f, 0.72f),
                TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -64f), new Vector2(-24f, 30f));

            rows = new List<RewardRowView>();
            for (int i = 0; i < 6; i++)
            {
                RectTransform rowRoot = DemoUiFactory.Rect(
                    "ui_panel_reward_row_" + i, panel.transform, new Vector2(0f, 1f), new Vector2(1f, 1f),
                    new Vector2(0.5f, 1f), new Vector2(0f, -106f - i * 72f), new Vector2(-32f, 62f));
                Image rowBg = rowRoot.gameObject.AddComponent<Image>();
                rowBg.color = new Color(0.13f, 0.17f, 0.24f, 0.95f);
                rowBg.raycastTarget = false;
                rowBg.maskable = false;
                RewardRowView row = rowRoot.gameObject.AddComponent<RewardRowView>();
                Image icon = DemoUiFactory.Image(
                    "ui_image_collected_reward_value", rowRoot, Sprite("UI_icon_gold.png"), Color.white,
                    new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                    new Vector2(12f, 0f), new Vector2(72f, 52f), false, true);
                TMP_Text amount = DemoUiFactory.Text(
                    "ui_text_collected_amount_value", rowRoot, "x0", 23f, Color.white,
                    TextAlignmentOptions.Right, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f),
                    new Vector2(-16f, 0f), new Vector2(-100f, 0f), FontStyles.Bold);
                SerializedObject rowSo = new SerializedObject(row);
                Set(rowSo, "ui_image_collected_reward_value", icon);
                Set(rowSo, "ui_text_collected_amount_value", amount);
                rowSo.ApplyModifiedPropertiesWithoutUndo();
                rows.Add(row);
            }
        }

        private static void CreateBottomActions(
            RectTransform parent,
            out Button spinButton,
            out Button leaveButton,
            out TMP_Text leaveHint)
        {
            spinButton = DemoUiFactory.Button(
                "ui_button_spin", parent, Sprite("UI_button_orange_standard.png"), Color.white, "SPIN",
                new Vector2(0.38f, 0f), new Vector2(0f, 56f), new Vector2(300f, 90f));
            leaveButton = DemoUiFactory.Button(
                "ui_button_leave", parent, Sprite("UI_button_grey_standard.png"), new Color(0.24f, 0.53f, 0.72f), "COLLECT & LEAVE",
                new Vector2(0.62f, 0f), new Vector2(0f, 56f), new Vector2(330f, 90f));
            leaveHint = DemoUiFactory.Text(
                "ui_text_leave_hint_value", parent, "UNLOCKS AT SAFE ZONES", 15f, new Color(0.56f, 0.64f, 0.74f),
                TextAlignmentOptions.Center, new Vector2(0.62f, 0f), new Vector2(0.62f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 6f), new Vector2(360f, 26f), FontStyles.Bold);
        }

        private static void CreateResultPopup(
            Transform parent,
            out GameObject popup,
            out RectTransform animator,
            out TMP_Text title,
            out TMP_Text message,
            out Image icon,
            out Button primaryButton,
            out TMP_Text primaryButtonText)
        {
            Image overlay = DemoUiFactory.Image(
                "ui_popup_result", parent, null, new Color(0f, 0f, 0f, 0.82f),
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, true);
            DemoUiFactory.Stretch(overlay.rectTransform, 0f, 0f, 0f, 0f);
            popup = overlay.gameObject;

            animator = DemoUiFactory.Rect(
                "ui_transform_popup_animator", overlay.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 720f));
            Image panel = animator.gameObject.AddComponent<Image>();
            panel.sprite = Sprite("ui_card_frame_12px_neutral.png");
            panel.type = Image.Type.Sliced;
            panel.color = new Color(0.08f, 0.02f, 0.025f, 0.99f);
            panel.raycastTarget = true;

            Image glow = DemoUiFactory.Image(
                "ui_image_popup_glow", animator, Sprite("star_flash_alpha.png"), new Color(0.74f, 0.05f, 0.03f, 0.28f),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 10f), new Vector2(620f, 620f), false, true);
            glow.transform.localRotation = Quaternion.Euler(0f, 0f, 8f);

            title = DemoUiFactory.Text(
                "ui_text_result_title_value", animator, "OH NO, THE BOMB EXPLODED!", 34f, Color.white,
                TextAlignmentOptions.Center, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -34f), new Vector2(-48f, 62f), FontStyles.Bold);
            message = DemoUiFactory.Text(
                "ui_text_result_message_value", animator, "All rewards from this run were lost.", 20f,
                new Color(0.82f, 0.85f, 0.9f), TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -98f), new Vector2(-70f, 64f));

            Image card = DemoUiFactory.Image(
                "ui_image_result_card", animator, Sprite("ui_card_frame_gardient.png"), new Color(0.85f, 0.08f, 0.05f, 1f),
                new Vector2(0.5f, 0.52f), new Vector2(0.5f, 0.52f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(250f, 390f), false, true);
            icon = DemoUiFactory.Image(
                "ui_image_result_icon_value", card.transform, Sprite("ui_card_icon_death.png"), Color.white,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(220f, 260f), false, true);

            primaryButton = DemoUiFactory.Button(
                "ui_button_result_primary", animator, Sprite("UI_button_grey_standard.png"), new Color(0.72f, 0.12f, 0.08f),
                "RESTART", new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(330f, 92f));
            primaryButtonText = primaryButton.GetComponentInChildren<TMP_Text>(true);
        }

        private static void Set(SerializedObject serializedObject, string propertyName, Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null) throw new System.InvalidOperationException("Missing serialized property: " + propertyName);
            property.objectReferenceValue = value;
        }

        private static void SetList<T>(SerializedObject serializedObject, string propertyName, List<T> values) where T : Object
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null) throw new System.InvalidOperationException("Missing serialized list: " + propertyName);
            property.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }
    }
}
