namespace VertigoDemo
{
    public sealed class BombOutcome : ISpinOutcome
    {
        public void Resolve(SpinContext context)
        {
            context.Popups.PlayBombImpact();
            context.Popups.ShowBomb(
                context.BombIcon,
                context.CurrencyReward.Icon,
                context.ReviveCost,
                context.RunState.Currency >= context.ReviveCost,
                context.RestartRun,
                context.CurrencyRevive);
        }
    }
}
