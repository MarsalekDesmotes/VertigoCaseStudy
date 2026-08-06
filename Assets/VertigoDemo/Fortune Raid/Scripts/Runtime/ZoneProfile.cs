using UnityEngine;

namespace VertigoDemo
{
    public sealed class ZoneProfile
    {
        public ZoneType Type { get; }
        public string TitleKey { get; }
        public Color AccentColor { get; }
        public bool CanCashOut { get; }
        public bool HasEntranceTransition { get; }
        public ZoneTrailStyle TrailStyle { get; }

        public ZoneProfile(
            ZoneType type,
            string titleKey,
            Color accentColor,
            bool canCashOut,
            bool hasEntranceTransition,
            ZoneTrailStyle trailStyle)
        {
            Type = type;
            TitleKey = titleKey;
            AccentColor = accentColor;
            CanCashOut = canCashOut;
            HasEntranceTransition = hasEntranceTransition;
            TrailStyle = trailStyle;
        }
    }
}
