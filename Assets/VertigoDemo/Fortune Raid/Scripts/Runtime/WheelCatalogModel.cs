using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace VertigoDemo
{
    [CreateAssetMenu(menuName = "Vertigo Demo/Wheel Catalog", fileName = "wheel_catalog")]
    public sealed class WheelCatalogModel : ScriptableObject
    {
        [SerializeField] private WheelDefinitionModel normalWheel;
        [SerializeField] private WheelDefinitionModel safeWheel;
        [FormerlySerializedAs("rotatingSuperWheels")]
        [SerializeField] private List<WheelDefinitionModel> rotatingGoldenWheels =
            new List<WheelDefinitionModel>();

        public WheelDefinitionModel ForZone(ZoneType type, int zone)
        {
            switch (type)
            {
                case ZoneType.Safe:
                    return safeWheel;
                case ZoneType.Golden:
                    int cycle = Mathf.Max(0, zone / 30 - 1);
                    return rotatingGoldenWheels[cycle % rotatingGoldenWheels.Count];
                default:
                    return normalWheel;
            }
        }

#if UNITY_EDITOR
        public void EditorConfigure(
            WheelDefinitionModel normal,
            WheelDefinitionModel safe,
            List<WheelDefinitionModel> goldenWheels)
        {
            normalWheel = normal;
            safeWheel = safe;
            rotatingGoldenWheels = goldenWheels;
        }
#endif
    }
}
