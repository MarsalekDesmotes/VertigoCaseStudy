using System;
using UnityEngine;

namespace VertigoDemo
{
    public interface IPopup
    {
        void ShowReward(RewardDefinitionModel reward, int amount, Action continueAction);
        void ShowBomb(
            Sprite bombIcon,
            Sprite reviveCurrencyIcon,
            int reviveCost,
            bool canAffordRevive,
            Action giveUpAction,
            Action currencyReviveAction);
        void PlayBombImpact();
        void ShowCollected(Sprite chestIcon, int rewardKinds, Action restartAction);
    }
}
