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
            this.monsterFactory = monsterFactory ?? throw new ArgumentNullException(nameof(monsterFactory));
            this.itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
            this.gameLogger = gameLogger ?? throw new ArgumentNullException(nameof(gameLogger));
        }

        public void StartNewGame()
        {
            InitializePlayer();
            CurrentState = GameState.Exploring;
            DungeonLevel = 1;
            gameLogger.Clear();
            LogWelcomeMessage();

            AddStartingItems();
        }

        private void InitializePlayer()
        {
            Player = new Player
            {
                Level = 1,
                Experience = 0,
                Gold = 0,
                Health = Player.MaxHealth,
            };
        }

        private void LogWelcomeMessage()
        {
            gameLogger.AddMessage("Welcome to the dungeon!");
        }

        private void AddStartingItems()
        {
            var startingPotion1 = CreateHealthPotion();
            var startingPotion2 = CreateHealthPotion();

            Player.AddItem(startingPotion1);
            Player.AddItem(startingPotion2);
        }

        private Potion CreateHealthPotion()
        {
            return new Potion
            {
                Name = "Health Potion",
                Description = "Restores 30 HP",
                HealAmount = 30,
                Value = 20
            };
        }

        public void ExploreNext()
        {
            ValidateGameState(GameState.Exploring);

            int encounterChance = Random.Shared.Next(100);
            if (encounterChance < 70)
            {
                EncounterMonster();
            }
            else
            {
                FindTreasure();
            }
        }

        private void ValidateGameState(GameState expected)
        {
            if (CurrentState != expected)
            {
                throw new InvalidOperationException($"Cannot perform action in current state: {CurrentState}. Expected state: {expected}.");
            }
        }

        public void AttackMonster()
        {
            if (CurrentMonster == null || !CurrentMonster.IsAlive)
            {
                gameLogger.AddMessage("No monster to attack.");
                return;
            }

            int playerDamage = CalculatePlayerDamage();
            CurrentMonster.TakeDamage(playerDamage);
            gameLogger.AddMessage($"You deal {playerDamage} damage to {CurrentMonster.Name}!");

            if (!CurrentMonster.IsAlive)
            {
                HandleMonsterDefeat();
                return;
            }

            int monsterDamage = CalculateMonsterDamage();
            Player.TakeDamage(monsterDamage);
            gameLogger.AddMessage($"{CurrentMonster.Name} deals {monsterDamage} damage to you!");

            if (!Player.IsAlive)
            {
                HandlePlayerDefeat();
            }
        }

        private int CalculatePlayerDamage()
        {
            int minDamage = Math.Max(0, Player.TotalAttack - 3);
            int maxDamage = Player.TotalAttack + 3;
            return Random.Shared.Next(minDamage, maxDamage + 1);
        }

        private int CalculateMonsterDamage()
        {
            if (CurrentMonster == null) return 0;
            int minDamage = Math.Max(0, CurrentMonster.Attack - 2);
            int maxDamage = CurrentMonster.Attack + 2;
            return Random.Shared.Next(minDamage, maxDamage + 1);
        }

        private void HandleMonsterDefeat()
        {
            DefeatMonster();
        }

        private void HandlePlayerDefeat()
        {
            CurrentState = GameState.GameOver;
            gameLogger.AddMessage("You have been defeated!");
        }

        public void UseItem(Item item)
        {
            if (item == null)
            {
                gameLogger.AddMessage("No item selected.");
                return;
            }

            if (!Player.Inventory.Contains(item))
            {
                gameLogger.AddMessage($"You don't have {item.Name} in your inventory.");
                return;
            }

            if (item is Potion potion)
            {
                Player.Heal(potion.HealAmount);
                Player.RemoveItem(item);
                gameLogger.AddMessage($"You used {item.Name} and restored {potion.HealAmount} HP!");
            }
            else
            {
                gameLogger.AddMessage($"Cannot use item: {item.Name}");
            }
        }

        public void EquipItem(Item item)
        {
            if (item == null)
            {
                gameLogger.AddMessage("No item selected.");
                return;
            }

            if (!Player.Inventory.Contains(item))
            {
                gameLogger.AddMessage($"You don't have {item.Name} in your inventory.");
                return;
            }

            if (Player.CanEquip(item))
            {
                Player.EquipItem(item);
                gameLogger.AddMessage($"You equipped {item.Name}!");
            }
            else
            {
                gameLogger.AddMessage($"Cannot equip {item.Name}.");
            }
        }

        public void SellItem(Item item)
        {
            if (item == null)
            {
                gameLogger.AddMessage("No item selected.");
                return;
            }

            if (!Player.Inventory.Contains(item))
            {
                gameLogger.AddMessage($"You don't have {item.Name} in your inventory.");
                return;
            }

            int sellValue = CalculateSellValue(item);
            Player.Gold += sellValue;
            Player.RemoveItem(item);
            gameLogger.AddMessage($"You sold {item.Name} for {sellValue} gold!");
        }

        private int CalculateSellValue(Item item)
        {
            return item.Value / 2;
        }

        public void ChangeState(GameState newState)
        {
            if (!Enum.IsDefined(typeof(GameState), newState))
            {
                throw new ArgumentOutOfRangeException(nameof(newState), "Invalid game state.");
            }

            CurrentState = newState;
        }

        private void EncounterMonster()
        {
            CurrentMonster = monsterFactory.CreateMonster(DungeonLevel);
            if (CurrentMonster == null)
            {
                gameLogger.AddMessage("Failed to encounter a monster.");
                return;
            }

            CurrentState = GameState.Combat;
            gameLogger.AddMessage($"You encounter a {CurrentMonster.Name} {CurrentMonster.Emoji}!");
        }

        private void DefeatMonster()
        {
            if (CurrentMonster == null)
            {
                gameLogger.AddMessage("No monster to defeat.");
                return;
            }

            GrantRewardsFromMonster(CurrentMonster);

            var loot = CurrentMonster.DropLoot();
            if (loot != null)
            {
                Player.AddItem(loot);
                gameLogger.AddMessage($"You found {loot.Name}!");
            }

            TryLevelUpPlayer();

            CurrentMonster = null;
            CurrentState = GameState.Exploring;

            TryDescendDungeon();
        }

        private void GrantRewardsFromMonster(Monster monster)
        {
            Player.Experience += monster.ExperienceReward;
            Player.Gold += monster.GoldReward;
            gameLogger.AddMessage($"You defeated {monster.Name}! Gained {monster.ExperienceReward} XP and {monster.GoldReward} gold!");
        }

        private void TryLevelUpPlayer()
        {
            while (Player.CanLevelUp)
            {
                var oldLevel = Player.Level;
                Player.LevelUp();
                gameLogger.AddMessage($"Level up! You are now level {Player.Level}!");
            }
        }

        private void TryDescendDungeon()
        {
            if (Random.Shared.Next(100) < 20)
            {
                DungeonLevel++;
                gameLogger.AddMessage($"You descend deeper into the dungeon! Floor {DungeonLevel}");
            }
        }

        private void FindTreasure()
        {
            int goldFound = CalculateGoldFound();
            Player.Gold += goldFound;
            gameLogger.AddMessage($"You found a treasure chest with {goldFound} gold!");

            if (Random.Shared.Next(100) < 40)
            {
                var item = itemFactory.CreateRandomItem();
                if (item != null)
                {
                    Player.AddItem(item);
                    gameLogger.AddMessage($"The chest also contained {item.Name}!");
                }
            }
        }

        private int CalculateGoldFound()
        {
            return Random.Shared.Next(5, 20) * DungeonLevel;
        }
    }
}
