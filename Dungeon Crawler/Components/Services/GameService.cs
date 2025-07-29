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

    public interface IMonsterFactory
    {
        Monster CreateMonster(int dungeonLevel);
        List<Monster> GetMonsterTemplates();
    }

    public interface IItemFactory
    {
        List<Weapon> GetWeaponTemplates();
        List<Armor> GetArmorTemplates();
        List<Potion> GetPotionTemplates();
        Item CreateRandomItem();
    }

    public interface IGameLogger
    {
        List<string> Messages { get; }
        void AddMessage(string message);
        void Clear();
    }

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

    public class MonsterFactory : IMonsterFactory
    {
        private readonly List<Monster> monsterTemplates = new()
        {
            new Monster { Name = "Goblin", Emoji = "👺", Health = 30, MaxHealth = 30, Attack = 8, Defense = 2, ExperienceReward = 15, GoldReward = 10 },
            new Monster { Name = "Skeleton", Emoji = "💀", Health = 40, MaxHealth = 40, Attack = 10, Defense = 3, ExperienceReward = 20, GoldReward = 15 },
            new Monster { Name = "Orc", Emoji = "👹", Health = 60, MaxHealth = 60, Attack = 15, Defense = 5, ExperienceReward = 30, GoldReward = 25 },
            new Monster { Name = "Ogre", Emoji = "🧌", Health = 80, MaxHealth = 80, Attack = 20, Defense = 7, ExperienceReward = 40, GoldReward = 35 },
            new Monster { Name = "Dragon", Emoji = "🐲", Health = 150, MaxHealth = 150, Attack = 30, Defense = 10, ExperienceReward = 100, GoldReward = 100 }
        };

        public Monster CreateMonster(int dungeonLevel)
        {
            var maxIndex = Math.Min(dungeonLevel, monsterTemplates.Count);
            var template = monsterTemplates[Random.Shared.Next(maxIndex)];

            return new Monster
            {
                Name = template.Name,
                Emoji = template.Emoji,
                Health = template.Health + (dungeonLevel - 1) * 5,
                MaxHealth = template.MaxHealth + (dungeonLevel - 1) * 5,
                Attack = template.Attack + (dungeonLevel - 1) * 2,
                Defense = template.Defense + (dungeonLevel - 1),
                ExperienceReward = template.ExperienceReward + (dungeonLevel - 1) * 5,
                GoldReward = template.GoldReward + (dungeonLevel - 1) * 3,
                PossibleLoot = new List<Item>()
            };
        }

        public List<Monster> GetMonsterTemplates()
        {
            return new List<Monster>(monsterTemplates);
        }
    }

    public class ItemFactory : IItemFactory
    {
        private readonly List<Weapon> weaponTemplates = new()
        {
            new Weapon { Name = "Rusty Sword", Description = "Old blade", Bonus = 3, Value = 25 },
            new Weapon { Name = "Steel Sword", Description = "Quality weapon", Bonus = 8, Value = 75 },
            new Weapon { Name = "Magic Sword", Description = "Glows with magic", Bonus = 15, Value = 200 },
            new Weapon { Name = "Excalibur", Description = "Legendary blade", Bonus = 25, Value = 500 }
        };

        private readonly List<Armor> armorTemplates = new()
        {
            new Armor { Name = "Leather Armor", Description = "Basic protection", Bonus = 2, Value = 30 },
            new Armor { Name = "Chain Mail", Description = "Metal protection", Bonus = 5, Value = 80 },
            new Armor { Name = "Plate Armor", Description = "Heavy protection", Bonus = 10, Value = 180 },
            new Armor { Name = "Dragon Scale", Description = "Legendary defense", Bonus = 18, Value = 400 }
        };

        private readonly List<Potion> potionTemplates = new()
        {
            new Potion { Name = "Health Potion", Description = "Restores 30 HP", HealAmount = 30, Value = 20 },
            new Potion { Name = "Greater Health Potion", Description = "Restores 60 HP", HealAmount = 60, Value = 40 },
            new Potion { Name = "Supreme Health Potion", Description = "Restores 100 HP", HealAmount = 100, Value = 80 }
        };

        public List<Weapon> GetWeaponTemplates() => new(weaponTemplates);
        public List<Armor> GetArmorTemplates() => new(armorTemplates);
        public List<Potion> GetPotionTemplates() => new(potionTemplates);

        public Item CreateRandomItem()
        {
            var itemType = Random.Shared.Next(3);
            return itemType switch
            {
                0 => weaponTemplates[Random.Shared.Next(weaponTemplates.Count)],
                1 => armorTemplates[Random.Shared.Next(armorTemplates.Count)],
                _ => potionTemplates[Random.Shared.Next(potionTemplates.Count)]
            };
        }
    }

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