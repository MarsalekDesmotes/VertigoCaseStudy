using System;
using System.Collections.Generic;

namespace VertigoDemo
{
    public interface IWheelScreenView
    {
        event Action SpinPressed;
        event Action LeavePressed;

        bool IsWheelSpinning { get; }

        void BindZone(int zone, ZoneProfile profile, WheelDefinitionModel wheel);
        void BindRewards(IReadOnlyList<CollectedRewardModel> rewards);
        void SetBusy(bool isBusy, bool canLeave);
        void SpinWheel(int selectedIndex, Action onComplete);
    }
}
