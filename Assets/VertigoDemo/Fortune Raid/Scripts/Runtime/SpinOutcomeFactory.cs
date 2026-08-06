namespace VertigoDemo
{
    public static class SpinOutcomeFactory
    {
        public static ISpinOutcome Create(
            WheelSliceDefinitionModel slice,
            int zone,
            IRewardRules rewardRules)
        {
            if (slice.IsBomb)
            {
                return new BombOutcome();
            }

            int amount = rewardRules.CalculateAmount(slice, zone);
            return new RewardOutcome(slice.Reward, amount);
        }
    }
}
