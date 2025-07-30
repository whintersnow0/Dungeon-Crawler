namespace BlazorDungeon.Services
{
    public interface IGameLogger
    {
        List<string> Messages { get; }
        void AddMessage(string message);
        void Clear();
    }
}