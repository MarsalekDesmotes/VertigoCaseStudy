using UnityEngine;

namespace VertigoDemo
{
    [CreateAssetMenu(menuName = "Vertigo Demo/Wheel Catalog", fileName = "wheel_catalog")]
    public sealed class WheelCatalog : ScriptableObject
    {
        [SerializeField] private WheelDefinition normalWheel;
        [SerializeField] private WheelDefinition safeWheel;
        [SerializeField] private WheelDefinition superWheel;

        public WheelDefinition ForZone(int zone)
        {
            switch (ZoneRules.GetZoneType(zone))
            {
                case ZoneType.Safe:
                    return safeWheel;
                case ZoneType.Super:
                    return superWheel;
                default:
                    return normalWheel;
            }
        }

#if UNITY_EDITOR
        public void EditorConfigure(WheelDefinition normal, WheelDefinition safe, WheelDefinition super)
        {
            normalWheel = normal;
            safeWheel = safe;
            superWheel = super;
        }
#endif
    }
}
