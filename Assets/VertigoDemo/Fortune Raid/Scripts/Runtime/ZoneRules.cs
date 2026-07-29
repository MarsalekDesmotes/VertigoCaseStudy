namespace VertigoDemo
{
    public static class ZoneRules
    {
        public static ZoneType GetZoneType(int zone)
        {
            if (zone <= 0)
            {
                return ZoneType.Normal;
            }

            if (zone % 30 == 0)
            {
                return ZoneType.Super;
            }

            return zone % 5 == 0 ? ZoneType.Safe : ZoneType.Normal;
        }

        public static bool CanLeave(int zone, bool isSpinning)
        {
            if (isSpinning)
            {
                return false;
            }

            ZoneType type = GetZoneType(zone);
            return type == ZoneType.Safe || type == ZoneType.Super;
        }
    }
}
