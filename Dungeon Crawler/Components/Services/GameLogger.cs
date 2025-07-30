using BlazorDungeon.Services;

namespace BlazorDungeon.Services
{
    public class GameLogger : IGameLogger
    {
        public List<string> Messages { get; } = new();

        public void AddMessage(string message)
        {
            Messages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            if (Messages.Count > 10)
            {
                Messages.RemoveAt(0);
            }
        }

        public void Clear()
        {
            Messages.Clear();
        }
    }
}