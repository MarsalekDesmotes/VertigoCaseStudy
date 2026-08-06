using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class RunLootPanelView : MonoBehaviour
    {
        [SerializeField] private List<RewardRowView> ui_reward_rows = new List<RewardRowView>();
        [SerializeField] private GameObject ui_panel_loot_empty_state;
        [SerializeField] private ScrollRect ui_scroll_rect_loot;
        [SerializeField] private RectTransform ui_transform_loot_scroll_content;

        private int lastBoundRewardCount;

        public void Bind(IReadOnlyList<CollectedRewardModel> rewards)
        {
            int rewardCount = rewards.Count;
            bool addedRewardType = rewardCount > lastBoundRewardCount;
            ui_panel_loot_empty_state.SetActive(rewardCount == 0);

            for (int i = 0; i < ui_reward_rows.Count; i++)
            {
                ui_reward_rows[i].Bind(i < rewardCount ? rewards[i] : null);
            }

            RefreshScroll(rewardCount, addedRewardType);
            lastBoundRewardCount = rewardCount;
        }

        public RectTransform FindFlightTarget(RewardDefinitionModel reward)
        {
            for (int i = 0; i < ui_reward_rows.Count; i++)
            {
                RewardRowView row = ui_reward_rows[i];
                if (row.Definition == reward)
                {
                    return row.FlightTarget;
                }
            }

            return null;
        }

        private void RefreshScroll(int rewardCount, bool addedRewardType)
        {
            const float rowPitch = 72f;
            const float finalRowTrim = 10f;
            Canvas.ForceUpdateCanvases();
            float viewportHeight = ui_scroll_rect_loot.viewport.rect.height;
            float requiredHeight = rewardCount > 0
                ? rewardCount * rowPitch - finalRowTrim
                : viewportHeight;
            Vector2 contentSize = ui_transform_loot_scroll_content.sizeDelta;
            contentSize.y = Mathf.Max(viewportHeight, requiredHeight);
            ui_transform_loot_scroll_content.sizeDelta = contentSize;
            Canvas.ForceUpdateCanvases();
            ui_scroll_rect_loot.StopMovement();

            Vector2 contentPosition = ui_transform_loot_scroll_content.anchoredPosition;
            if (rewardCount == 0)
            {
                contentPosition.y = 0f;
                ui_transform_loot_scroll_content.anchoredPosition = contentPosition;
            }
            else if (addedRewardType && requiredHeight > viewportHeight)
            {
                contentPosition.y = ui_transform_loot_scroll_content.rect.height - viewportHeight;
                ui_transform_loot_scroll_content.anchoredPosition = contentPosition;
            }
        }

    }
}
