using UnityEngine;

namespace VertigoDemo
{
    [CreateAssetMenu(menuName = "Vertigo Demo/Reward", fileName = "reward_")]
    public sealed class RewardDefinition : ScriptableObject
    {
        [SerializeField] private string id = "reward";
        [SerializeField] private string displayName = "Reward";
        [SerializeField] private Sprite icon;
        [SerializeField, Min(1)] private int baseAmount = 1;

        public string Id { get { return id; } }
        public string DisplayName { get { return displayName; } }
        public Sprite Icon { get { return icon; } }
        public int BaseAmount { get { return baseAmount; } }

#if UNITY_EDITOR
        public void EditorConfigure(string rewardId, string title, Sprite rewardIcon, int amount)
        {
            id = rewardId;
            displayName = title;
            icon = rewardIcon;
            baseAmount = Mathf.Max(1, amount);
        }
#endif
    }
}
