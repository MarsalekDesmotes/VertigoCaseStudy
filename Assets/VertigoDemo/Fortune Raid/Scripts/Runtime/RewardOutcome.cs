namespace VertigoDemo
{
    public sealed class RewardOutcome : ISpinOutcome
    {
        private readonly RewardDefinitionModel reward;
        private readonly int amount;

        public RewardOutcome(RewardDefinitionModel reward, int amount)
        {
            this.reward = reward;
            this.amount = amount;
        }

        public void Resolve(SpinContext context)
        {
            if (!context.RunState.TryAddReward(reward, amount))
            {
                context.AdvanceRun();
                return;
            }

            context.Screen.BindRewards(context.RunState.Rewards);
            context.Popups.ShowReward(reward, amount, context.AdvanceRun);
        }
    }
}
