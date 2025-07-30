using BlazorDungeon.Models;

namespace BlazorDungeon.Services
{
    public class GameService : IGameService
    {
        private readonly IMonsterFactory monsterFactory;
        private readonly IItemFactory itemFactory;
        private readonly IGameLogger gameLogger;

        public Player Player { get; private set; } = new();
        public Monster? CurrentMonster { get; private set; }
        public GameState CurrentState { get; private set; } = GameState.Menu;
        public int DungeonLevel { get; private set; } = 1;
        public List<string> GameLog => gameLogger.Messages;

        public GameService(IMonsterFactory monsterFactory, IItemFactory itemFactory, IGameLogger gameLogger)
        {
            this.monsterFactory = monsterFactory;
            this.itemFactory = itemFactory;
            this.gameLogger = gameLogger;
        }

        public void StartNewGame()
        {
            Player = new Player();
            CurrentState = GameState.Exploring;
            DungeonLevel = 1;
            gameLogger.Clear();
            gameLogger.AddMessage("Welcome to the dungeon!");

            Player.AddItem(new Potion { Name = "Health Potion", Description = "Restores 30 HP", HealAmount = 30, Value = 20 });
            Player.AddItem(new Potion { Name = "Health Potion", Description = "Restores 30 HP", HealAmount = 30, Value = 20 });
        }

        public void ExploreNext()
        {
            if (Random.Shared.Next(100) < 70)
            {
                EncounterMonster();
            }
            else
            {
                FindTreasure();
            }
        }

        public void AttackMonster()
        {
            if (CurrentMonster == null || !CurrentMonster.IsAlive) return;

            var playerDamage = Random.Shared.Next(Player.TotalAttack - 3, Player.TotalAttack + 4);
            CurrentMonster.TakeDamage(playerDamage);
            gameLogger.AddMessage($"You deal {playerDamage} damage to {CurrentMonster.Name}!");

            if (!CurrentMonster.IsAlive)
            {
                DefeatMonster();
                return;
            }

            var monsterDamage = Random.Shared.Next(CurrentMonster.Attack - 2, CurrentMonster.Attack + 3);
            Player.TakeDamage(monsterDamage);
            gameLogger.AddMessage($"{CurrentMonster.Name} deals {monsterDamage} damage to you!");

            if (!Player.IsAlive)
            {
                CurrentState = GameState.GameOver;
                gameLogger.AddMessage("You have been defeated!");
            }
        }

        public void UseItem(Item item)
        {
            if (item is Potion potion)
            {
                Player.Heal(potion.HealAmount);
                Player.RemoveItem(item);
                gameLogger.AddMessage($"You used {item.Name} and restored {potion.HealAmount} HP!");
            }
        }

        public void EquipItem(Item item)
        {
            if (Player.CanEquip(item))
            {
                Player.EquipItem(item);
                gameLogger.AddMessage($"You equipped {item.Name}!");
            }
        }

        public void SellItem(Item item)
        {
            Player.Gold += item.Value / 2;
            Player.RemoveItem(item);
            gameLogger.AddMessage($"You sold {item.Name} for {item.Value / 2} gold!");
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
        }

        private void EncounterMonster()
        {
            CurrentMonster = monsterFactory.CreateMonster(DungeonLevel);
            CurrentState = GameState.Combat;
            gameLogger.AddMessage($"You encounter a {CurrentMonster.Name} {CurrentMonster.Emoji}!");
        }

        private void DefeatMonster()
        {
            if (CurrentMonster == null) return;

            Player.Experience += CurrentMonster.ExperienceReward;
            Player.Gold += CurrentMonster.GoldReward;
            gameLogger.AddMessage($"You defeated {CurrentMonster.Name}! Gained {CurrentMonster.ExperienceReward} XP and {CurrentMonster.GoldReward} gold!");

            var loot = CurrentMonster.DropLoot();
            if (loot != null)
            {
                Player.AddItem(loot);
                gameLogger.AddMessage($"You found {loot.Name}!");
            }

            if (Player.CanLevelUp)
            {
                var oldLevel = Player.Level;
                Player.LevelUp();
                gameLogger.AddMessage($"Level up! You are now level {Player.Level}!");
            }

            CurrentMonster = null;
            CurrentState = GameState.Exploring;

            if (Random.Shared.Next(100) < 20)
            {
                DungeonLevel++;
                gameLogger.AddMessage($"You descend deeper into the dungeon! Floor {DungeonLevel}");
            }
        }

        private void FindTreasure()
        {
            var goldFound = Random.Shared.Next(5, 20) * DungeonLevel;
            Player.Gold += goldFound;
            gameLogger.AddMessage($"You found a treasure chest with {goldFound} gold!");

            if (Random.Shared.Next(100) < 40)
            {
                var item = itemFactory.CreateRandomItem();
                Player.AddItem(item);
                gameLogger.AddMessage($"The chest also contained {item.Name}!");
            }
        }
    }
}