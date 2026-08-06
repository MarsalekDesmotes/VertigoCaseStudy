using System;
using System.Collections.Generic;

namespace VertigoDemo
{
    [Serializable]
    public sealed class CollectedRewardModel
    {
        public RewardDefinitionModel Definition;
        public int Amount;

        public CollectedRewardModel(RewardDefinitionModel definition, int amount)
        {
            Definition = definition;
            Amount = amount;
        }
    }

    public sealed class RunStateModel
    {
        private readonly List<CollectedRewardModel> rewards = new List<CollectedRewardModel>();
        private readonly RewardDefinitionModel currencyReward;
        private readonly int startingCurrency;

        public int Zone { get; private set; }
        public int Currency { get; private set; }
        public IReadOnlyList<CollectedRewardModel> Rewards { get { return rewards; } }

        public RunStateModel(RewardDefinitionModel currencyReward, int startingCurrency)
        {
            this.currencyReward = currencyReward;
            this.startingCurrency = startingCurrency;
            Restart();
        }

        public bool TryAddReward(RewardDefinitionModel definition, int amount)
        {
            CollectedRewardModel existing = rewards.Find(item => item.Definition == definition);
            if (existing != null)
            {
                if (!definition.IsStackable)
                {
                    return false;
                }

                existing.Amount += amount;
            }
            else
            {
                rewards.Add(new CollectedRewardModel(definition, amount));
            }

            if (definition == currencyReward)
            {
                Currency += amount;
            }

            return true;
        }

        public void AdvanceZone()
        {
            Zone++;
        }

        public bool TrySpendCurrency(int amount)
        {
            if (Currency < amount)
            {
                return false;
            }

            Currency -= amount;
            return true;
        }

        public void Restart()
        {
            rewards.Clear();
            Zone = 1;
            Currency = startingCurrency;
        }
    }
}
