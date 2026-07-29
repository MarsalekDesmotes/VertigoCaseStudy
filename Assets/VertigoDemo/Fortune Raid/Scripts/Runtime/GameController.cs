using UnityEngine;

namespace VertigoDemo
{
    // it runs the game loop
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private WheelCatalog wheelCatalog;
        [SerializeField] private GameScreenView gameScreenView;
        [SerializeField] private Sprite bombIcon;
        [SerializeField] private Sprite collectedChestIcon;
        [Header("Revive")]
        [SerializeField] private RewardDefinition reviveCurrencyDefinition;
        [SerializeField, Min(1)] private int reviveCost = 25;
        [SerializeField, Min(0)] private int startingReviveCurrency = 100;

        private RunState runState;
        private WheelDefinition activeWheel;

        private void Awake()
        {
            runState = new RunState(startingReviveCurrency);
            gameScreenView.SpinPressed += HandleSpinPressed;
            gameScreenView.LeavePressed += HandleLeavePressed;
            RefreshScreen();
        }

        private void OnDestroy()
        {
            gameScreenView.SpinPressed -= HandleSpinPressed;
            gameScreenView.LeavePressed -= HandleLeavePressed;
        }

        private void HandleSpinPressed()
        {
            if (gameScreenView.Wheel.IsSpinning || activeWheel.Slices.Count == 0)
            {
                return;
            }

            // it keeps reward odds on the backend
            int selectedIndex = Random.Range(0, activeWheel.Slices.Count);
            StartSpin(selectedIndex);
        }

        private void StartSpin(int selectedIndex)
        {
            gameScreenView.SetInteraction(true, false);
            gameScreenView.Wheel.Spin(selectedIndex, () => ResolveSpin(selectedIndex));
        }

        private void ResolveSpin(int selectedIndex)
        {
            WheelSliceDefinition result = activeWheel.Slices[selectedIndex];
            if (result.IsBomb)
            {
                gameScreenView.PlayBombImpact();
                gameScreenView.ShowBomb(
                    bombIcon,
                    reviveCurrencyDefinition.Icon,
                    reviveCost,
                    runState.Currency >= reviveCost,
                    RestartRun,
                    HandleCurrencyRevive,
                    HandleRewardedRevive);
                return;
            }

            int amount = WheelSliceView.CalculateAmount(result, runState.Zone);
            runState.AddReward(result.Reward, amount);
            gameScreenView.BindRewards(runState.Rewards);
            gameScreenView.ShowReward(result.Reward, amount, AdvanceRun);
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

        private void HandleRewardedRevive()
        {
            AdvanceRun();
        }

        private void HandleLeavePressed()
        {
            if (!ZoneRules.CanLeave(runState.Zone, gameScreenView.Wheel.IsSpinning))
            {
                return;
            }

            gameScreenView.ShowCollected(collectedChestIcon, runState.Rewards.Count, RestartRun);
        }

        private void RestartRun()
        {
            runState.Restart();
            RefreshScreen();
        }

        private void RefreshScreen()
        {
            activeWheel = wheelCatalog.ForZone(runState.Zone);
            gameScreenView.BindZone(runState.Zone, activeWheel);
            gameScreenView.BindRewards(runState.Rewards);
            gameScreenView.SetInteraction(false, ZoneRules.CanLeave(runState.Zone, false));
        }

    }
}
