using UnityEngine;

namespace VertigoDemo
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private WheelCatalog wheelCatalog;
        [SerializeField] private GameScreenView gameScreenView;
        [SerializeField] private Sprite bombIcon;
        [SerializeField] private Sprite collectedChestIcon;

        private readonly RunState runState = new RunState();
        private WheelDefinition activeWheel;

        private void Awake()
        {
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
            if (gameScreenView.Wheel.IsSpinning || activeWheel == null || activeWheel.Slices.Count == 0)
            {
                return;
            }

            int selectedIndex = Random.Range(0, activeWheel.Slices.Count);
            StartSpin(selectedIndex);
        }

        public void TriggerDemoSpin()
        {
            if (gameScreenView.Wheel.IsSpinning || activeWheel == null)
            {
                return;
            }

            for (int i = 0; i < activeWheel.Slices.Count; i++)
            {
                if (!activeWheel.Slices[i].IsBomb)
                {
                    StartSpin(i);
                    return;
                }
            }
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
                runState.LoseRewards();
                gameScreenView.BindRewards(runState.Rewards);
                gameScreenView.ShowBomb(bombIcon, RestartRun);
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

        private void OnValidate()
        {
            if (gameScreenView == null) gameScreenView = FindObjectOfType<GameScreenView>(true);
        }
    }
}
