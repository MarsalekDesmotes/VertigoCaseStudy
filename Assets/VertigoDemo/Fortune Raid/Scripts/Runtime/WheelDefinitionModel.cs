using System;
using System.Collections.Generic;
using UnityEngine;

namespace VertigoDemo
{
    [Serializable]
    public sealed class WheelSliceDefinitionModel
    {
        [SerializeField] private bool isBomb;
        [SerializeField] private RewardDefinitionModel reward;
        [SerializeField, Min(1)] private int amountMultiplier = 1;

        public bool IsBomb { get { return isBomb; } }
        public RewardDefinitionModel Reward { get { return reward; } }
        public int AmountMultiplier { get { return amountMultiplier; } }

        public WheelSliceDefinitionModel()
        {
        }

        public WheelSliceDefinitionModel(
            bool bomb,
            RewardDefinitionModel rewardDefinition,
            int multiplier)
        {
            isBomb = bomb;
            reward = rewardDefinition;
            amountMultiplier = Mathf.Max(1, multiplier);
        }
    }

    [CreateAssetMenu(menuName = "Vertigo Demo/Wheel", fileName = "wheel_")]
    public sealed class WheelDefinitionModel : ScriptableObject
    {
        [SerializeField] private ZoneType zoneType;
        [SerializeField] private Sprite wheelBase;
        [SerializeField] private Sprite indicator;
        [SerializeField] private List<WheelSliceDefinitionModel> slices = new List<WheelSliceDefinitionModel>();

        public ZoneType ZoneType { get { return zoneType; } }
        public Sprite WheelBase { get { return wheelBase; } }
        public Sprite Indicator { get { return indicator; } }
        public IReadOnlyList<WheelSliceDefinitionModel> Slices { get { return slices; } }

        public bool ValidateBombs()
        {
            int bombCount = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i] != null && slices[i].IsBomb)
                {
                    bombCount++;
                }
            }

            int requiredBombs = zoneType == ZoneType.Normal ? 1 : 0;
            return bombCount == requiredBombs;
        }

        private void OnValidate()
        {
            if (!ValidateBombs())
            {
                int bombCount = 0;
                for (int i = 0; i < slices.Count; i++)
                {
                    if (slices[i] != null && slices[i].IsBomb)
                    {
                        bombCount++;
                    }
                }

                int requiredBombs = zoneType == ZoneType.Normal ? 1 : 0;
                Debug.LogError(
                    name + ": " + zoneType + " wheel requires exactly " +
                    requiredBombs + " bomb slice(s), found " + bombCount + ".",
                    this);
            }
        }

#if UNITY_EDITOR
        public void EditorConfigure(
            ZoneType type,
            Sprite baseSprite,
            Sprite indicatorSprite,
            List<WheelSliceDefinitionModel> items)
        {
            zoneType = type;
            wheelBase = baseSprite;
            indicator = indicatorSprite;
            slices = items;
        }
#endif
    }
}
