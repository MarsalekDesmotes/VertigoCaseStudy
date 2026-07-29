using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    // it binds the main screen
    public sealed class GameScreenView : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private WheelView ui_wheel_view;
        [SerializeField] private TMP_Text ui_text_zone_value;
        [SerializeField] private TMP_Text ui_text_zone_type_value;
        [SerializeField] private Button ui_button_spin;
        [SerializeField] private Button ui_button_leave;

        [Header("Sections")]
        [SerializeField] private ResultPopupView ui_result_popup_view;
        [SerializeField] private BombPopupView ui_bomb_popup_view;
        [SerializeField] private RunLootPanelView ui_run_loot_panel_view;
        [SerializeField] private ZoneTrailView ui_zone_trail_view;
        [SerializeField] private GoldenTransitionView ui_golden_transition_view;

        private bool leaveWasAvailable;
        private bool requestedSpinning;
        private bool requestedCanLeave;
        private int lastBoundZone = -1;

        public WheelView Wheel => ui_wheel_view;
        public event Action SpinPressed;
        public event Action LeavePressed;

        private void Awake()
        {
            ui_button_spin.onClick.AddListener(HandleSpinPressed);
            ui_button_leave.onClick.AddListener(HandleLeavePressed);
        }

        private void OnDestroy()
        {
            ui_button_spin.onClick.RemoveListener(HandleSpinPressed);
            ui_button_leave.onClick.RemoveListener(HandleLeavePressed);
        }

        public void BindZone(int zone, WheelDefinition wheel)
        {
            ZoneType type = ZoneRules.GetZoneType(zone);
            ui_text_zone_value.text = "ZONE " + zone;
            ui_text_zone_type_value.text = type == ZoneType.Normal
                ? "RISK ZONE"
                : type == ZoneType.Safe ? "SAFE ZONE" : "SUPER ZONE";
            ui_text_zone_type_value.color = type == ZoneType.Normal
                ? new Color(1f, 0.46f, 0.2f)
                : type == ZoneType.Safe
                    ? new Color(0.52f, 0.84f, 1f)
                    : new Color(1f, 0.78f, 0.12f);

            ui_wheel_view.Bind(wheel, zone);
            ui_zone_trail_view.Bind(zone);
            bool newZone = zone != lastBoundZone;
            lastBoundZone = zone;

            if (newZone && type == ZoneType.Super)
            {
                ui_golden_transition_view.Play(
                    zone,
                    () => ui_wheel_view.PlayZoneReveal(true),
                    HandleGoldenTransitionComplete);
                ApplyInteractionState();
            }
            else
            {
                ui_wheel_view.PlayZoneReveal(false);
            }

            RectTransform zoneTransform = ui_text_zone_value.rectTransform;
            DOTween.Kill(zoneTransform, false);
            zoneTransform.localScale = new Vector3(0.90f, 0.90f, 1f);
            zoneTransform
                .DOScale(Vector3.one, 0.24f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .SetTarget(zoneTransform);
        }

        public void BindRewards(IReadOnlyList<CollectedReward> rewards)
        {
            ui_run_loot_panel_view.Bind(rewards);
        }

        public void SetInteraction(bool spinning, bool canLeave)
        {
            requestedSpinning = spinning;
            requestedCanLeave = canLeave;
            ApplyInteractionState();
        }

        public void ShowReward(
            RewardDefinition reward,
            int amount,
            Action continueAction)
        {
            ui_result_popup_view.ShowReward(
                reward,
                amount,
                ui_run_loot_panel_view.FindFlightTarget(reward),
                continueAction);
        }

        public void ShowBomb(
            Sprite bombIcon,
            Sprite reviveCurrencyIcon,
            int reviveCost,
            bool canAffordRevive,
            Action giveUpAction,
            Action currencyReviveAction,
            Action rewardedReviveAction)
        {
            ui_bomb_popup_view.Show(
                bombIcon,
                reviveCurrencyIcon,
                reviveCost,
                canAffordRevive,
                giveUpAction,
                currencyReviveAction,
                rewardedReviveAction);
        }

        public void PlayBombImpact()
        {
            ui_bomb_popup_view.PlayImpact();
        }

        public void ShowCollected(
            Sprite chestIcon,
            int rewardKinds,
            Action restartAction)
        {
            ui_result_popup_view.ShowCollected(chestIcon, rewardKinds, restartAction);
        }

        public void HideResult()
        {
            ui_bomb_popup_view.ResetState();
            ui_result_popup_view.Hide();
        }

        private void ApplyInteractionState()
        {
            bool transitionPlaying = ui_golden_transition_view.IsPlaying;
            bool canSpin = !requestedSpinning && !transitionPlaying;
            bool canLeave = requestedCanLeave && !transitionPlaying;
            ui_button_spin.interactable = canSpin;
            ui_button_leave.interactable = canLeave;
            if (canLeave && !leaveWasAvailable)
            {
                RectTransform leaveAnimator =
                    (RectTransform)ui_button_leave.transform.parent;
                DOTween.Kill(leaveAnimator, false);
                leaveAnimator.localScale = Vector3.one;
                leaveAnimator
                    .DOPunchScale(
                        new Vector3(0.10f, 0.10f, 0f),
                        0.34f,
                        8,
                        0.55f)
                    .SetUpdate(true)
                    .SetTarget(leaveAnimator);
            }

            leaveWasAvailable = canLeave;
        }

        private void HandleGoldenTransitionComplete()
        {
            ApplyInteractionState();
            ui_text_zone_type_value.rectTransform
                .DOPunchScale(
                    new Vector3(0.12f, 0.12f, 0f),
                    0.28f,
                    8,
                    0.55f)
                .SetUpdate(true)
                .SetTarget(ui_text_zone_type_value.rectTransform);
        }

        private void HandleSpinPressed()
        {
            SpinPressed?.Invoke();
        }

        private void HandleLeavePressed()
        {
            LeavePressed?.Invoke();
        }

    }
}
