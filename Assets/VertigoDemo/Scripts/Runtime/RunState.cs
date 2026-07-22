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

    public sealed class RunState
    {
        private readonly List<CollectedReward> rewards = new List<CollectedReward>();

        public int Zone { get; private set; }
        public IReadOnlyList<CollectedReward> Rewards { get { return rewards; } }

        public RunState()
        {
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
                rewards.Add(new CollectedReward(definition, amount));
            }
            else
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

        public void Restart()
        {
            rewards.Clear();
            Zone = 1;
        }
    }
}
