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
        private static GameObject BuildGamePrefab(WheelCatalogModel catalog)
        {
            GameObject root = new GameObject(
                "ui_screen_game",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(GameScreenView),
                typeof(GameRoot));
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

            DemoUiFactory.Image(
                "ui_image_loot_cool_glow", root.transform, Sprite("star_glow_alpha.png"),
                new Color(0.08f, 0.42f, 0.95f, 0.065f),
                new Vector2(0.84f, 0.50f), new Vector2(0.84f, 0.50f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(560f, 760f), false, true);

            RectTransform safeArea = DemoUiFactory.Rect(
                "ui_panel_safe_area", root.transform, Vector2.zero, Vector2.one,
                new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            DemoUiFactory.Stretch(safeArea, 42f, 34f, 42f, 28f);

            CreateHeader(safeArea, out TMP_Text zoneText, out TMP_Text zoneTypeText,
                out List<Image> zoneNodes, out List<TMP_Text> zoneLabels);
            WheelView wheelView = CreateWheel(safeArea, out Button spinButton);
            CreateRewardPanel(
                safeArea,
                CountUniqueRewards(catalog),
                out List<RewardRowView> rewardRows,
                out GameObject lootEmptyState,
                out ScrollRect lootScrollRect,
                out RectTransform lootScrollContent);
            CreateBottomActions(safeArea, out Button leaveButton);
            CreateSuperTransition(
                root.transform,
                out GameObject superTransition,
                out CanvasGroup superTransitionCanvasGroup,
                out TMP_Text superTransitionTitle,
                out TMP_Text superTransitionPunchline);
            CreateResultPopup(root.transform, out GameObject popup, out RectTransform popupAnimator,
                out CanvasGroup resultCanvasGroup, out Image popupPanel,
                out Image resultCard, out Image resultCardGlow, out Image resultCardBorder,
                out TMP_Text resultTitle, out TMP_Text resultMessage, out TMP_Text resultReward, out Image resultIcon,
                out Button resultButton, out TMP_Text resultButtonText, out GameObject bombActions,
                out Button giveUpButton, out Button currencyReviveButton, out Button rewardedReviveButton,
                out Image reviveCurrencyIcon, out TMP_Text reviveCostText);
            CanvasGroup giveUpCanvasGroup = giveUpButton.gameObject.AddComponent<CanvasGroup>();
            CanvasGroup currencyReviveCanvasGroup =
                currencyReviveButton.gameObject.AddComponent<CanvasGroup>();
            rewardedReviveButton.gameObject.SetActive(false);
            Image bombImpactFlash = DemoUiFactory.Image(
                "ui_image_bomb_impact_flash_value", root.transform, null,
                new Color(0.92f, 0.015f, 0.01f, 0f),
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, Vector2.zero, false, true);
            DemoUiFactory.Stretch(bombImpactFlash.rectTransform, 0f, 0f, 0f, 0f);
            bombImpactFlash.gameObject.SetActive(false);
            Image rewardFly = DemoUiFactory.Image(
                "ui_image_reward_fly_value", root.transform, Sprite("UI_icon_gold.png"), Color.white,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(150f, 150f), false, true);
            RewardFlyView rewardFlyView =
                rewardFly.gameObject.AddComponent<RewardFlyView>();
            SerializedObject rewardFlySo = new SerializedObject(rewardFlyView);
            Set(rewardFlySo, "ui_image_reward_fly_value", rewardFly);
            rewardFlySo.ApplyModifiedPropertiesWithoutUndo();
            rewardFly.gameObject.SetActive(false);

            ZoneTrailView zoneTrailView =
                zoneNodes[0].transform.parent.gameObject.AddComponent<ZoneTrailView>();
            SerializedObject zoneTrailSo = new SerializedObject(zoneTrailView);
            SetList(zoneTrailSo, "ui_zone_nodes", zoneNodes);
            SetList(zoneTrailSo, "ui_zone_node_labels_value", zoneLabels);
            Set(zoneTrailSo, "ui_sprite_zone_current", Sprite("ui_card_panel_zone_current_white.png"));
            Set(zoneTrailSo, "ui_sprite_zone_coming", Sprite("ui_card_panel_zone_coming.png"));
            Set(zoneTrailSo, "ui_sprite_zone_safe", Sprite("ui_card_panel_zone_current.png"));
            Set(zoneTrailSo, "ui_sprite_zone_golden", Sprite("ui_card_panel_zone_super.png"));
            zoneTrailSo.ApplyModifiedPropertiesWithoutUndo();

            RunLootPanelView runLootPanelView =
                lootScrollRect.gameObject.AddComponent<RunLootPanelView>();
            SerializedObject runLootSo = new SerializedObject(runLootPanelView);
            SetList(runLootSo, "ui_reward_rows", rewardRows);
            Set(runLootSo, "ui_panel_loot_empty_state", lootEmptyState);
            Set(runLootSo, "ui_scroll_rect_loot", lootScrollRect);
            Set(runLootSo, "ui_transform_loot_scroll_content", lootScrollContent);
            runLootSo.ApplyModifiedPropertiesWithoutUndo();

            GoldenTransitionView goldenTransitionView =
                superTransition.AddComponent<GoldenTransitionView>();
            SerializedObject goldenTransitionSo =
                new SerializedObject(goldenTransitionView);
            Set(
                goldenTransitionSo,
                "ui_canvas_group_super_transition",
                superTransitionCanvasGroup);
            Set(
                goldenTransitionSo,
                "ui_text_super_transition_title_value",
                superTransitionTitle);
            Set(
                goldenTransitionSo,
                "ui_text_super_transition_punchline_value",
                superTransitionPunchline);
            goldenTransitionSo.ApplyModifiedPropertiesWithoutUndo();

            BombActionsView bombActionsView = bombActions.AddComponent<BombActionsView>();
            SerializedObject bombActionsSo = new SerializedObject(bombActionsView);
            Set(bombActionsSo, "ui_button_bomb_give_up", giveUpButton);
            Set(bombActionsSo, "ui_button_bomb_currency_revive", currencyReviveButton);
            Set(bombActionsSo, "ui_button_bomb_rewarded_revive", rewardedReviveButton);
            Set(bombActionsSo, "ui_canvas_group_bomb_give_up", giveUpCanvasGroup);
            Set(bombActionsSo, "ui_canvas_group_bomb_currency_revive", currencyReviveCanvasGroup);
            Set(bombActionsSo, "ui_image_bomb_revive_currency_value", reviveCurrencyIcon);
            Set(bombActionsSo, "ui_text_bomb_revive_cost_value", reviveCostText);
            Set(bombActionsSo, "ui_text_bomb_give_up_label", giveUpButton.GetComponentInChildren<TMP_Text>(true));
            Set(bombActionsSo, "ui_text_bomb_revive_label", currencyReviveButton.GetComponentInChildren<TMP_Text>(true));
            bombActionsSo.ApplyModifiedPropertiesWithoutUndo();

            PopupView popupView = popup.AddComponent<PopupView>();
            SerializedObject popupSo = new SerializedObject(popupView);
            Set(popupSo, "ui_transform_popup_animator", popupAnimator);
            Set(popupSo, "ui_canvas_group_result", resultCanvasGroup);
            Set(popupSo, "ui_image_popup_panel_value", popupPanel);
            Set(popupSo, "ui_image_result_card_value", resultCard);
            Set(popupSo, "ui_image_result_card_glow_value", resultCardGlow);
            Set(popupSo, "ui_image_result_card_border_value", resultCardBorder);
            Set(popupSo, "ui_text_result_title_value", resultTitle);
            Set(popupSo, "ui_text_result_message_value", resultMessage);
            Set(popupSo, "ui_text_result_reward_value", resultReward);
            Set(popupSo, "ui_image_result_icon_value", resultIcon);
            Set(popupSo, "ui_reward_fly_view", rewardFlyView);
            Set(popupSo, "ui_button_result_primary", resultButton);
            Set(popupSo, "ui_text_result_primary_value", resultButtonText);
            Set(popupSo, "ui_bomb_actions_view", bombActionsView);
            Set(popupSo, "ui_sprite_result_speed_lines", Sprite("star_flash_alpha.png"));
            Set(popupSo, "ui_sprite_result_special_shine", Sprite("ui_vfx_offer_shine.tga"));
            Set(popupSo, "ui_wheel_view", wheelView);
            Set(popupSo, "ui_image_bomb_impact_flash_value", bombImpactFlash);
            popupSo.ApplyModifiedPropertiesWithoutUndo();

            GameScreenView screen = root.GetComponent<GameScreenView>();
            SerializedObject screenSo = new SerializedObject(screen);
            Set(screenSo, "ui_wheel_view", wheelView);
            Set(screenSo, "ui_text_zone_value", zoneText);
            Set(screenSo, "ui_text_zone_type_value", zoneTypeText);
            Set(screenSo, "ui_button_spin", spinButton);
            Set(screenSo, "ui_button_leave", leaveButton);
            Set(screenSo, "ui_popup_view", popupView);
            Set(screenSo, "ui_run_loot_panel_view", runLootPanelView);
            Set(screenSo, "ui_zone_trail_view", zoneTrailView);
            Set(screenSo, "ui_golden_transition_view", goldenTransitionView);
            screenSo.ApplyModifiedPropertiesWithoutUndo();

            GameRoot gameRoot = root.GetComponent<GameRoot>();
            SerializedObject gameRootSo = new SerializedObject(gameRoot);
            Set(gameRootSo, "wheelCatalog", catalog);
            Set(gameRootSo, "gameScreenView", screen);
            Set(gameRootSo, "bombIcon", Sprite("ui_card_icon_death.png"));
            Set(gameRootSo, "collectedChestIcon", Sprite("UI_icon_chest_gold_nolight.png"));
            Set(gameRootSo, "reviveCurrencyDefinition",
                AssetDatabase.LoadAssetAtPath<RewardDefinitionModel>(Data + "/reward_gold.asset"));
            gameRootSo.ApplyModifiedPropertiesWithoutUndo();

            GameObject eventSystem = new GameObject("ui_system_events", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(root.transform, false);
            superTransition.SetActive(false);
            popup.SetActive(false);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, GamePrefabPath);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static int CountUniqueRewards(WheelCatalogModel catalog)
        {
            HashSet<RewardDefinitionModel> rewards = new HashSet<RewardDefinitionModel>();
            for (int zone = 1; zone <= 90; zone++)
            {
                WheelDefinitionModel wheel = catalog.ForZone(zone);
                if (wheel == null)
                {
                    continue;
                }

                for (int i = 0; i < wheel.Slices.Count; i++)
                {
                    WheelSliceDefinitionModel slice = wheel.Slices[i];
                    if (slice != null && !slice.IsBomb && slice.Reward != null)
                    {
                        rewards.Add(slice.Reward);
                    }
                }
            }

            return Mathf.Max(1, rewards.Count);
        }

        private static void Set(SerializedObject serializedObject, string propertyName, Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                throw new System.InvalidOperationException(
                    "Missing serialized property: " + propertyName);
            }
            property.objectReferenceValue = value;
        }

        private static void SetList<T>(SerializedObject serializedObject, string propertyName, List<T> values) where T : Object
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                throw new System.InvalidOperationException(
                    "Missing serialized list: " + propertyName);
            }
            property.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }
    }
}
