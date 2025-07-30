using BlazorDungeon.Models;

namespace BlazorDungeon.Services
{
    public interface IGameService
    {
        Player Player { get; }
        Monster? CurrentMonster { get; }
        GameState CurrentState { get; }
        int DungeonLevel { get; }
        List<string> GameLog { get; }

        void StartNewGame();
        void ExploreNext();
        void AttackMonster();
        void UseItem(Item item);
        void EquipItem(Item item);
        void SellItem(Item item);
        void ChangeState(GameState newState);
    }
}