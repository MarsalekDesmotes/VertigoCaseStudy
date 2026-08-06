namespace VertigoDemo
{
    public interface IZoneRules
    {
        ZoneProfile GetProfile(int zone);
        bool CanLeave(int zone, bool isBusy);
    }
}
