namespace VertigoDemo
{
    public interface ILocalization
    {
        string Get(string key);
        string Format(string key, params object[] args);
    }
}
