using System;
using System.Collections.Generic;
using UnityEngine;

namespace VertigoDemo
{
    [Serializable]
    public sealed class WheelSliceDefinition
    {
        [SerializeField] private bool isBomb;
        [SerializeField] private RewardDefinition reward;
        [SerializeField, Min(1)] private int amountMultiplier = 1;

        public bool IsBomb { get { return isBomb; } }
        public RewardDefinition Reward { get { return reward; } }
        public int AmountMultiplier { get { return amountMultiplier; } }

#if UNITY_EDITOR
        public WheelSliceDefinition(bool bomb, RewardDefinition rewardDefinition, int multiplier)
        {
            isBomb = bomb;
            reward = rewardDefinition;
            amountMultiplier = Mathf.Max(1, multiplier);
        }
#endif
    }

    [CreateAssetMenu(menuName = "Vertigo Demo/Wheel", fileName = "wheel_")]
    public sealed class WheelDefinition : ScriptableObject
    {
        [SerializeField] private ZoneType zoneType;
        [SerializeField] private Sprite wheelBase;
        [SerializeField] private Sprite indicator;
        [SerializeField] private List<WheelSliceDefinition> slices = new List<WheelSliceDefinition>();

        public ZoneType ZoneType { get { return zoneType; } }
        public Sprite WheelBase { get { return wheelBase; } }
        public Sprite Indicator { get { return indicator; } }
        public IReadOnlyList<WheelSliceDefinition> Slices { get { return slices; } }

#if UNITY_EDITOR
        public void EditorConfigure(ZoneType type, Sprite baseSprite, Sprite indicatorSprite, List<WheelSliceDefinition> items)
        {
            zoneType = type;
            wheelBase = baseSprite;
            indicator = indicatorSprite;
            slices = items;
        }
#endif
    }
}
