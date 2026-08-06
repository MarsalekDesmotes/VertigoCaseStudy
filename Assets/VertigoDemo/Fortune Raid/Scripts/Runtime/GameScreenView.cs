using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class GameScreenView : MonoBehaviour, IWheelScreenView, IPopup
    {
        [Header("Core")]
        [SerializeField] private WheelView ui_wheel_view;
        [SerializeField] private TMP_Text ui_text_zone_value;
        [SerializeField] private TMP_Text ui_text_zone_type_value;
        [SerializeField] private Button ui_button_spin;
        [SerializeField] private Button ui_button_leave;

        [Header("Sections")]
        [FormerlySerializedAs("ui_result_popup_view")]
        [SerializeField] private PopupView ui_popup_view;
        [SerializeField] private RunLootPanelView ui_run_loot_panel_view;
        [SerializeField] private ZoneTrailView ui_zone_trail_view;
        [SerializeField] private GoldenTransitionView ui_golden_transition_view;

        private ILocalization localization;
        private IZoneRules zoneRules;
        private IRewardRules rewardRules;
        private bool leaveWasAvailable;
        private bool requestedBusy;
        private bool requestedCanLeave;
        private int lastBoundZone = -1;

        public bool IsWheelSpinning { get { return ui_wheel_view.IsSpinning; } }
        public event Action SpinPressed;
        public event Action LeavePressed;

        public void Configure(
            ILocalization localizationService,
            IZoneRules zoneRulesService,
            IRewardRules rewardRulesService)
        {
            localization = localizationService;
            zoneRules = zoneRulesService;
            rewardRules = rewardRulesService;
            ui_popup_view.Configure(localization);
            ui_golden_transition_view.Configure(localization);
        }

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

        public void BindZone(int zone, ZoneProfile profile, WheelDefinitionModel wheel)
        {
            ui_text_zone_value.text = localization.Format(LocalizationKeys.ZoneLabel, zone);
            ui_text_zone_type_value.text = localization.Get(profile.TitleKey);
            ui_text_zone_type_value.color = profile.AccentColor;

            ui_wheel_view.Bind(wheel, zone, rewardRules);
            ui_zone_trail_view.Bind(zone, zoneRules);
            bool newZone = zone != lastBoundZone;
            lastBoundZone = zone;

            if (newZone && profile.HasEntranceTransition)
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

        public void BindRewards(IReadOnlyList<CollectedRewardModel> rewards)
        {
            ui_run_loot_panel_view.Bind(rewards);
        }

        public void SetBusy(bool isBusy, bool canLeave)
        {
            requestedBusy = isBusy;
            requestedCanLeave = canLeave;
            ApplyInteractionState();
        }

        public void SpinWheel(int selectedIndex, Action onComplete)
        {
            ui_wheel_view.Spin(selectedIndex, onComplete);
        }

        public void ShowReward(
            RewardDefinitionModel reward,
            int amount,
            Action continueAction)
        {
            ui_popup_view.ShowReward(
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
            Action currencyReviveAction)
        {
            ui_popup_view.ShowBomb(
                bombIcon,
                reviveCurrencyIcon,
                reviveCost,
                canAffordRevive,
                giveUpAction,
                currencyReviveAction);
        }

        public void PlayBombImpact()
        {
            ui_popup_view.PlayImpact();
        }

        public void ShowCollected(
            Sprite chestIcon,
            int rewardKinds,
            Action restartAction)
        {
            ui_popup_view.ShowCollected(chestIcon, rewardKinds, restartAction);
        }

        private void ApplyInteractionState()
        {
            bool transitionPlaying = ui_golden_transition_view.IsPlaying;
            bool canSpin = !requestedBusy && !transitionPlaying;
            bool canLeave = requestedCanLeave && !transitionPlaying;
            ui_button_spin.interactable = canSpin;
            ui_button_leave.interactable = canLeave;
            if (canLeave && !leaveWasAvailable)
            {
                RectTransform leaveAnimator =
                    (RectTransform)ui_button_leave.transform.parent;
                DOTween.Kill(leaveAnimator, false);
                leaveAnimator.localScale = Vector3.one;
                UiTween.PunchScale(
                    leaveAnimator,
                    new Vector3(0.10f, 0.10f, 0f),
                    0.34f);
            }

            leaveWasAvailable = canLeave;
        }

        private void HandleGoldenTransitionComplete()
        {
            ApplyInteractionState();
            UiTween.PunchScale(
                ui_text_zone_type_value.rectTransform,
                new Vector3(0.12f, 0.12f, 0f),
                0.28f);
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
