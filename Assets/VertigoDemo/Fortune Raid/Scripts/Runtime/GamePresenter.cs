using System;
using UnityEngine;

namespace VertigoDemo
{
    public sealed class GamePresenter : IDisposable
    {
        private readonly IWheelScreenView screen;
        private readonly IPopup popups;
        private readonly WheelCatalogModel wheelCatalog;
        private readonly RunStateModel runState;
        private readonly IZoneRules zoneRules;
        private readonly IRewardRules rewardRules;
        private readonly Sprite collectedChestIcon;
        private readonly int reviveCost;
        private readonly SpinContext spinContext;

        private WheelDefinitionModel activeWheel;

        public GamePresenter(
            IWheelScreenView screen,
            IPopup popups,
            WheelCatalogModel wheelCatalog,
            RunStateModel runState,
            IZoneRules zoneRules,
            IRewardRules rewardRules,
            RewardDefinitionModel currencyReward,
            Sprite bombIcon,
            Sprite collectedChestIcon,
            int reviveCost)
        {
            this.screen = screen;
            this.popups = popups;
            this.wheelCatalog = wheelCatalog;
            this.runState = runState;
            this.zoneRules = zoneRules;
            this.rewardRules = rewardRules;
            this.collectedChestIcon = collectedChestIcon;
            this.reviveCost = reviveCost;
            spinContext = new SpinContext(
                popups,
                screen,
                runState,
                currencyReward,
                bombIcon,
                reviveCost,
                AdvanceRun,
                RestartRun,
                HandleCurrencyRevive);
        }

        public void Start()
        {
            screen.SpinPressed += HandleSpinPressed;
            screen.LeavePressed += HandleLeavePressed;
            RefreshScreen();
        }

        public void Dispose()
        {
            screen.SpinPressed -= HandleSpinPressed;
            screen.LeavePressed -= HandleLeavePressed;
        }

        private void HandleSpinPressed()
        {
            if (screen.IsWheelSpinning)
            {
                return;
            }

            int selectedIndex = UnityEngine.Random.Range(0, activeWheel.Slices.Count);
            screen.SetBusy(true, false);
            screen.SpinWheel(selectedIndex, () => ResolveSpin(selectedIndex));
        }

        private void ResolveSpin(int selectedIndex)
        {
            WheelSliceDefinitionModel result = activeWheel.Slices[selectedIndex];
            ISpinOutcome outcome = SpinOutcomeFactory.Create(
                result,
                runState.Zone,
                rewardRules);
            outcome.Resolve(spinContext);
        }

        private void AdvanceRun()
        {
            runState.AdvanceZone();
            RefreshScreen();
        }

        private void HandleCurrencyRevive()
        {
            if (!runState.TrySpendCurrency(reviveCost))
            {
                return;
            }

            AdvanceRun();
        }

        private void HandleLeavePressed()
        {
            if (!zoneRules.CanLeave(runState.Zone, screen.IsWheelSpinning))
            {
                return;
            }

            popups.ShowCollected(collectedChestIcon, runState.Rewards.Count, RestartRun);
        }

        private void RestartRun()
        {
            runState.Restart();
            RefreshScreen();
        }

        private void RefreshScreen()
        {
            ZoneProfile profile = zoneRules.GetProfile(runState.Zone);
            activeWheel = wheelCatalog.ForZone(profile.Type, runState.Zone);
            screen.BindZone(runState.Zone, profile, activeWheel);
            screen.BindRewards(runState.Rewards);
            screen.SetBusy(false, zoneRules.CanLeave(runState.Zone, false));
        }
    }
}
