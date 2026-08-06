using UnityEngine;

namespace VertigoDemo
{
    public sealed class ZoneRules : IZoneRules
    {
        private static readonly ZoneProfile Normal = new ZoneProfile(
            ZoneType.Normal,
            LocalizationKeys.ZoneRisk,
            new Color(1f, 0.46f, 0.2f),
            canCashOut: false,
            hasEntranceTransition: false,
            ZoneTrailStyle.Coming);

        private static readonly ZoneProfile Safe = new ZoneProfile(
            ZoneType.Safe,
            LocalizationKeys.ZoneSafe,
            new Color(0.52f, 0.84f, 1f),
            canCashOut: true,
            hasEntranceTransition: false,
            ZoneTrailStyle.Safe);

        private static readonly ZoneProfile Golden = new ZoneProfile(
            ZoneType.Golden,
            LocalizationKeys.ZoneGolden,
            new Color(1f, 0.78f, 0.12f),
            canCashOut: true,
            hasEntranceTransition: true,
            ZoneTrailStyle.Golden);

        public ZoneProfile GetProfile(int zone)
        {
            if (zone % 30 == 0)
            {
                return Golden;
            }

            return zone % 5 == 0 ? Safe : Normal;
        }

        public bool CanLeave(int zone, bool isBusy)
        {
            if (isBusy)
            {
                return false;
            }

            return GetProfile(zone).CanCashOut;
        }
    }
}
