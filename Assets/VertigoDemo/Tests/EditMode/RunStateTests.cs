using NUnit.Framework;
using UnityEngine;

namespace VertigoDemo.Tests
{
    public sealed class RunStateTests
    {
        [Test]
        public void Restart_ClearsRewardsAndReturnsToZoneOne()
        {
            RewardDefinition reward = ScriptableObject.CreateInstance<RewardDefinition>();
            RunState state = new RunState();
            state.AddReward(reward, 5);
            state.AdvanceZone();

            state.Restart();

            Assert.AreEqual(1, state.Zone);
            Assert.AreEqual(0, state.Rewards.Count);
            Object.DestroyImmediate(reward);
        }

        [Test]
        public void AddReward_MergesSameDefinition()
        {
            RewardDefinition reward = ScriptableObject.CreateInstance<RewardDefinition>();
            RunState state = new RunState();
            state.AddReward(reward, 3);
            state.AddReward(reward, 4);

            Assert.AreEqual(1, state.Rewards.Count);
            Assert.AreEqual(7, state.Rewards[0].Amount);
            Object.DestroyImmediate(reward);
        }
    }
}
