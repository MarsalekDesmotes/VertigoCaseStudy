using UnityEngine;

namespace VertigoDemo
{
    public sealed class RewardRules : IRewardRules
    {
        public int CalculateAmount(WheelSliceDefinitionModel definition, int zone)
        {
            if (definition.IsBomb)
            {
                return 0;
            }

            RewardDefinitionModel reward = definition.Reward;
            if (!reward.IsStackable)
            {
                return reward.BaseAmount;
            }

            return reward.BaseAmount * definition.AmountMultiplier * Mathf.Max(1, zone);
        }
    }
}
