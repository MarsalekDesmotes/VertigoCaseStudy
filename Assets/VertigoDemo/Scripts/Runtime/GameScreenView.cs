using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class GameScreenView : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private WheelView ui_wheel_view;
        [SerializeField] private TMP_Text ui_text_zone_value;
        [SerializeField] private TMP_Text ui_text_zone_type_value;
        [SerializeField] private Button ui_button_spin;
        [SerializeField] private Button ui_button_leave;
        [SerializeField] private TMP_Text ui_text_leave_hint_value;

        [Header("Collected rewards")]
        [SerializeField] private List<RewardRowView> ui_reward_rows = new List<RewardRowView>();

        [Header("Zone trail")]
        [SerializeField] private List<Image> ui_zone_nodes = new List<Image>();
        [SerializeField] private List<TMP_Text> ui_zone_node_labels_value = new List<TMP_Text>();

        [Header("Result popup")]
        [SerializeField] private GameObject ui_popup_result;
        [SerializeField] private RectTransform ui_transform_popup_animator;
        [SerializeField] private TMP_Text ui_text_result_title_value;
        [SerializeField] private TMP_Text ui_text_result_message_value;
        [SerializeField] private Image ui_image_result_icon_value;
        [SerializeField] private Button ui_button_result_primary;
        [SerializeField] private TMP_Text ui_text_result_primary_value;

        private Action resultPrimaryAction;

        public WheelView Wheel { get { return ui_wheel_view; } }
        public event Action SpinPressed;
        public event Action LeavePressed;

        private void Awake()
        {
            ui_button_spin.onClick.AddListener(HandleSpinPressed);
            ui_button_leave.onClick.AddListener(HandleLeavePressed);
            ui_button_result_primary.onClick.AddListener(HandleResultPrimaryPressed);
        }

        private void OnDestroy()
        {
            ui_button_spin.onClick.RemoveListener(HandleSpinPressed);
            ui_button_leave.onClick.RemoveListener(HandleLeavePressed);
            ui_button_result_primary.onClick.RemoveListener(HandleResultPrimaryPressed);
        }

        public void BindZone(int zone, WheelDefinition wheel)
        {
            ZoneType type = ZoneRules.GetZoneType(zone);
            ui_text_zone_value.text = "ZONE " + zone;
            ui_text_zone_type_value.text = type == ZoneType.Normal ? "RISK ZONE" : type == ZoneType.Safe ? "SAFE ZONE" : "SUPER ZONE";
            ui_text_zone_type_value.color = type == ZoneType.Normal
                ? new Color(1f, 0.46f, 0.2f)
                : type == ZoneType.Safe ? new Color(0.52f, 0.84f, 1f) : new Color(1f, 0.78f, 0.12f);
            ui_wheel_view.Bind(wheel, zone);
            RefreshTrail(zone);
        }

        public void BindRewards(IReadOnlyList<CollectedReward> rewards)
        {
            for (int i = 0; i < ui_reward_rows.Count; i++)
            {
                ui_reward_rows[i].Bind(i < rewards.Count ? rewards[i] : null);
            }
        }

        public void SetInteraction(bool spinning, bool canLeave)
        {
            ui_button_spin.interactable = !spinning;
            ui_button_leave.interactable = canLeave;
            ui_text_leave_hint_value.text = canLeave ? "COLLECT YOUR LOOT" : "UNLOCKS AT SAFE ZONES";
        }

        public void ShowReward(RewardDefinition reward, int amount, Action continueAction)
        {
            ShowResult(
                "REWARD SECURED!",
                reward.DisplayName + "  x" + amount + "\nPush your luck in the next zone.",
                reward.Icon,
                "CONTINUE",
                continueAction,
                new Color(0.12f, 0.55f, 0.25f));
        }

        public void ShowBomb(Sprite bombIcon, Action restartAction)
        {
            ShowResult(
                "OH NO, THE BOMB EXPLODED!",
                "All rewards from this run were lost.\nStart again and try your luck.",
                bombIcon,
                "RESTART",
                restartAction,
                new Color(0.72f, 0.12f, 0.08f));
        }

        public void ShowCollected(Sprite chestIcon, int rewardKinds, Action restartAction)
        {
            ShowResult(
                "REWARDS COLLECTED",
                "You safely left with " + rewardKinds + " reward types.",
                chestIcon,
                "NEW RUN",
                restartAction,
                new Color(0.85f, 0.45f, 0.05f));
        }

        public void HideResult()
        {
            resultPrimaryAction = null;
            ui_popup_result.SetActive(false);
        }

        public void TriggerResultPrimaryForDemo()
        {
            HandleResultPrimaryPressed();
        }

        private void ShowResult(string title, string message, Sprite icon, string buttonLabel, Action action, Color buttonColor)
        {
            resultPrimaryAction = action;
            ui_text_result_title_value.text = title;
            ui_text_result_message_value.text = message;
            ui_image_result_icon_value.sprite = icon;
            ui_image_result_icon_value.preserveAspect = true;
            ui_text_result_primary_value.text = buttonLabel;
            ui_button_result_primary.image.color = buttonColor;
            ui_transform_popup_animator.localScale = Vector3.one;
            ui_popup_result.SetActive(true);
        }

        private void RefreshTrail(int zone)
        {
            int start = Mathf.Max(1, zone - 2);
            for (int i = 0; i < ui_zone_nodes.Count; i++)
            {
                int representedZone = start + i;
                ZoneType representedType = ZoneRules.GetZoneType(representedZone);
                ui_zone_node_labels_value[i].text = representedZone.ToString();
                ui_zone_nodes[i].color = representedZone == zone
                    ? Color.white
                    : representedType == ZoneType.Super ? new Color(1f, 0.72f, 0.08f)
                    : representedType == ZoneType.Safe ? new Color(0.45f, 0.75f, 0.95f)
                    : new Color(0.23f, 0.27f, 0.34f);
            }
        }

        private void HandleSpinPressed()
        {
            if (SpinPressed != null) SpinPressed.Invoke();
        }

        private void HandleLeavePressed()
        {
            if (LeavePressed != null) LeavePressed.Invoke();
        }

        private void HandleResultPrimaryPressed()
        {
            Action action = resultPrimaryAction;
            HideResult();
            if (action != null) action.Invoke();
        }

        private void OnValidate()
        {
            if (ui_wheel_view == null) ui_wheel_view = GetComponentInChildren<WheelView>(true);
            AutoAssign(ref ui_text_zone_value, "ui_text_zone_value");
            AutoAssign(ref ui_text_zone_type_value, "ui_text_zone_type_value");
            AutoAssign(ref ui_button_spin, "ui_button_spin");
            AutoAssign(ref ui_button_leave, "ui_button_leave");
            AutoAssign(ref ui_text_leave_hint_value, "ui_text_leave_hint_value");
            if (ui_reward_rows.Count == 0) ui_reward_rows.AddRange(GetComponentsInChildren<RewardRowView>(true));
            AutoAssign(ref ui_popup_result, "ui_popup_result");
            AutoAssign(ref ui_transform_popup_animator, "ui_transform_popup_animator");
            AutoAssign(ref ui_text_result_title_value, "ui_text_result_title_value");
            AutoAssign(ref ui_text_result_message_value, "ui_text_result_message_value");
            AutoAssign(ref ui_image_result_icon_value, "ui_image_result_icon_value");
            AutoAssign(ref ui_button_result_primary, "ui_button_result_primary");
            AutoAssign(ref ui_text_result_primary_value, "ui_text_result_primary_value");
        }

        private void AutoAssign<T>(ref T field, string objectName) where T : Component
        {
            if (field != null) return;
            Transform[] children = GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name == objectName)
                {
                    field = children[i].GetComponent<T>();
                    return;
                }
            }
        }

        private void AutoAssign(ref GameObject field, string objectName)
        {
            if (field != null) return;
            Transform[] children = GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name == objectName)
                {
                    field = children[i].gameObject;
                    return;
                }
            }
        }
    }
}
