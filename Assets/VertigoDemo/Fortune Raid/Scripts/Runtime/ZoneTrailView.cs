using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class ZoneTrailView : MonoBehaviour
    {
        [SerializeField] private List<Image> ui_zone_nodes = new List<Image>();
        [SerializeField] private List<TMP_Text> ui_zone_node_labels_value = new List<TMP_Text>();
        [SerializeField] private Sprite ui_sprite_zone_current;
        [SerializeField] private Sprite ui_sprite_zone_coming;
        [SerializeField] private Sprite ui_sprite_zone_safe;
        [SerializeField] private Sprite ui_sprite_zone_super;

        public void Bind(int zone)
        {
            int start = Mathf.Max(1, zone - 2);
            int count = Mathf.Min(ui_zone_nodes.Count, ui_zone_node_labels_value.Count);
            for (int i = 0; i < count; i++)
            {
                int representedZone = start + i;
                ZoneType representedType = ZoneRules.GetZoneType(representedZone);
                bool isCurrent = representedZone == zone;
                bool isPast = representedZone < zone;
                Image node = ui_zone_nodes[i];

                ui_zone_node_labels_value[i].text = representedZone.ToString();
                node.sprite = isCurrent
                    ? ui_sprite_zone_current
                    : representedType == ZoneType.Super ? ui_sprite_zone_super
                    : representedType == ZoneType.Safe ? ui_sprite_zone_safe
                    : ui_sprite_zone_coming;
                node.color = isCurrent
                    ? representedType == ZoneType.Super ? new Color(1f, 0.72f, 0.08f)
                    : representedType == ZoneType.Safe ? new Color(0.52f, 0.84f, 1f)
                    : Color.white
                    : isPast ? new Color(0.38f, 0.42f, 0.48f)
                    : new Color(0.20f, 0.24f, 0.32f);

                RectTransform nodeTransform = node.rectTransform;
                DOTween.Kill(nodeTransform, false);
                nodeTransform
                    .DOScale(isCurrent ? new Vector3(1.16f, 1.16f, 1f) : Vector3.one, 0.20f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true)
                    .SetTarget(nodeTransform);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < ui_zone_nodes.Count; i++)
            {
                DOTween.Kill(ui_zone_nodes[i].rectTransform, false);
            }
        }
    }
}
