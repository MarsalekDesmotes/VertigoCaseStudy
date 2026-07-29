using System.Collections.Generic;
using UnityEngine;

namespace VertigoDemo
{
    [CreateAssetMenu(menuName = "Vertigo Demo/Wheel Catalog", fileName = "wheel_catalog")]
    // it chooses the wheel for each zone
    public sealed class WheelCatalog : ScriptableObject
    {
        [SerializeField] private WheelDefinition normalWheel;
        [SerializeField] private WheelDefinition safeWheel;
        [SerializeField] private List<WheelDefinition> rotatingSuperWheels =
            new List<WheelDefinition>();

        public WheelDefinition ForZone(int zone)
        {
            switch (ZoneRules.GetZoneType(zone))
            {
                case ZoneType.Safe:
                    return safeWheel;
                case ZoneType.Super:
                    int cycle = Mathf.Max(0, zone / 30 - 1);
                    return rotatingSuperWheels[cycle % rotatingSuperWheels.Count];
                default:
                    return normalWheel;
            }
        }

#if UNITY_EDITOR
        public void EditorConfigure(
            WheelDefinition normal,
            WheelDefinition safe,
            List<WheelDefinition> superWheels)
        {
            normalWheel = normal;
            safeWheel = safe;
            rotatingSuperWheels = superWheels;
        }
#endif
    }
}
