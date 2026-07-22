using NUnit.Framework;

namespace VertigoDemo.Tests
{
    public sealed class ZoneRulesTests
    {
        [TestCase(1, ZoneType.Normal)]
        [TestCase(4, ZoneType.Normal)]
        [TestCase(5, ZoneType.Safe)]
        [TestCase(10, ZoneType.Safe)]
        [TestCase(29, ZoneType.Normal)]
        [TestCase(30, ZoneType.Super)]
        [TestCase(60, ZoneType.Super)]
        public void GetZoneType_ReturnsExpectedType(int zone, ZoneType expected)
        {
            Assert.AreEqual(expected, ZoneRules.GetZoneType(zone));
        }

        [Test]
        public void CanLeave_OnlyAllowsIdleSafeOrSuperZones()
        {
            Assert.IsFalse(ZoneRules.CanLeave(4, false));
            Assert.IsTrue(ZoneRules.CanLeave(5, false));
            Assert.IsTrue(ZoneRules.CanLeave(30, false));
            Assert.IsFalse(ZoneRules.CanLeave(5, true));
        }
    }
}
