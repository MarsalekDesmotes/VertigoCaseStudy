using System;
using UnityEngine;

namespace VertigoDemo
{
    public sealed class SpinContext
    {
        public IPopup Popups { get; }
        public IWheelScreenView Screen { get; }
        public RunStateModel RunState { get; }
        public RewardDefinitionModel CurrencyReward { get; }
        public Sprite BombIcon { get; }
        public int ReviveCost { get; }
        public Action AdvanceRun { get; }
        public Action RestartRun { get; }
        public Action CurrencyRevive { get; }

        public SpinContext(
            IPopup popups,
            IWheelScreenView screen,
            RunStateModel runState,
            RewardDefinitionModel currencyReward,
            Sprite bombIcon,
            int reviveCost,
            Action advanceRun,
            Action restartRun,
            Action currencyRevive)
        {
            Popups = popups;
            Screen = screen;
            RunState = runState;
            CurrencyReward = currencyReward;
            BombIcon = bombIcon;
            ReviveCost = reviveCost;
            AdvanceRun = advanceRun;
            RestartRun = restartRun;
            CurrencyRevive = currencyRevive;
        }
    }
}
