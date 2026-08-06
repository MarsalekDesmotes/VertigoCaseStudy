namespace VertigoDemo
{
    public interface IRewardRules
    {
        int CalculateAmount(WheelSliceDefinitionModel definition, int zone);
    }
}
