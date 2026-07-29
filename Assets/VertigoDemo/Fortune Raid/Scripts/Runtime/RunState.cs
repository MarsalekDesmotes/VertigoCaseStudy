using System;
using System.Collections.Generic;

namespace VertigoDemo
{
    [Serializable]
    public sealed class CollectedReward
    {
        public RewardDefinition Definition;
        public int Amount;

        public CollectedReward(RewardDefinition definition, int amount)
        {
            Definition = definition;
            Amount = amount;
        }
    }

    // it tracks the current run
    public sealed class RunState
    {
        private const int DefaultStartingCurrency = 100;
        private readonly List<CollectedReward> rewards = new List<CollectedReward>();

        public int Zone { get; private set; }
        public int Currency { get; private set; }
        public IReadOnlyList<CollectedReward> Rewards { get { return rewards; } }

        public RunState(int startingCurrency = DefaultStartingCurrency)
        {
            Currency = Math.Max(0, startingCurrency);
            Restart();
        }

        public void AddReward(RewardDefinition definition, int amount)
        {
            if (definition == null || amount <= 0)
            {
                return;
            }

            CollectedReward existing = rewards.Find(item => item.Definition == definition);
            if (existing == null)
            {
                rewards.Add(new CollectedReward(
                    definition,
                    definition.IsStackable ? amount : definition.BaseAmount));
            }
            else if (definition.IsStackable)
            {
                existing.Amount += amount;
            }
        }

        public void AdvanceZone()
        {
            Zone++;
        }

        public void LoseRewards()
        {
            rewards.Clear();
        }

        public bool TrySpendCurrency(int amount)
        {
            if (amount <= 0 || Currency < amount)
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
        }
    }
}
