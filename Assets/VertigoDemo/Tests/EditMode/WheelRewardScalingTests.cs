using NUnit.Framework;
using UnityEngine;

namespace VertigoDemo.Tests
{
    public sealed class WheelRewardScalingTests
    {
        [Test]
        public void CalculateAmount_IncreasesAtEveryZone()
        {
            RewardDefinition reward = ScriptableObject.CreateInstance<RewardDefinition>();
            reward.EditorConfigure("gold", "Gold", null, 10);
            WheelSliceDefinition slice = new WheelSliceDefinition(false, reward, 2);

            int zoneOne = WheelSliceView.CalculateAmount(slice, 1);
            int zoneTwo = WheelSliceView.CalculateAmount(slice, 2);

            Assert.AreEqual(20, zoneOne);
            Assert.AreEqual(40, zoneTwo);
            Object.DestroyImmediate(reward);
        }
    }
}
