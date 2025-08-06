using BlazorDungeon.Models;

namespace BlazorDungeon.Services
{
    public class MonsterFactory : IMonsterFactory
    {
        private readonly List<Monster> monsterTemplates = new()
        {
            new Monster { Name = "Rat", Emoji = "🐀", Health = 15, MaxHealth = 15, Attack = 4, Defense = 1, ExperienceReward = 8, GoldReward = 5 },
            new Monster { Name = "Bat", Emoji = "🦇", Health = 20, MaxHealth = 20, Attack = 6, Defense = 1, ExperienceReward = 10, GoldReward = 7 },
            new Monster { Name = "Spider", Emoji = "🕷️", Health = 25, MaxHealth = 25, Attack = 7, Defense = 2, ExperienceReward = 12, GoldReward = 8 },
            new Monster { Name = "Goblin", Emoji = "👺", Health = 30, MaxHealth = 30, Attack = 8, Defense = 2, ExperienceReward = 15, GoldReward = 10 },
            new Monster { Name = "Wolf", Emoji = "🐺", Health = 35, MaxHealth = 35, Attack = 9, Defense = 3, ExperienceReward = 17, GoldReward = 12 },
            new Monster { Name = "Skeleton", Emoji = "💀", Health = 40, MaxHealth = 40, Attack = 10, Defense = 3, ExperienceReward = 20, GoldReward = 15 },
            new Monster { Name = "Zombie", Emoji = "🧟", Health = 45, MaxHealth = 45, Attack = 11, Defense = 4, ExperienceReward = 22, GoldReward = 17 },
            new Monster { Name = "Bear", Emoji = "🐻", Health = 50, MaxHealth = 50, Attack = 12, Defense = 4, ExperienceReward = 25, GoldReward = 20 },
            new Monster { Name = "Troll", Emoji = "🧌", Health = 55, MaxHealth = 55, Attack = 13, Defense = 5, ExperienceReward = 27, GoldReward = 22 },
            new Monster { Name = "Orc", Emoji = "👹", Health = 60, MaxHealth = 60, Attack = 15, Defense = 5, ExperienceReward = 30, GoldReward = 25 },
            new Monster { Name = "Minotaur", Emoji = "🐂", Health = 70, MaxHealth = 70, Attack = 17, Defense = 6, ExperienceReward = 35, GoldReward = 30 },
            new Monster { Name = "Ogre", Emoji = "🧟‍♂️", Health = 80, MaxHealth = 80, Attack = 20, Defense = 7, ExperienceReward = 40, GoldReward = 35 },
            new Monster { Name = "Cyclops", Emoji = "👁️", Health = 90, MaxHealth = 90, Attack = 22, Defense = 8, ExperienceReward = 45, GoldReward = 40 },
            new Monster { Name = "Demon", Emoji = "👿", Health = 100, MaxHealth = 100, Attack = 24, Defense = 8, ExperienceReward = 50, GoldReward = 45 },
            new Monster { Name = "Lich", Emoji = "☠️", Health = 120, MaxHealth = 120, Attack = 26, Defense = 9, ExperienceReward = 60, GoldReward = 55 },
            new Monster { Name = "Hydra", Emoji = "🐍", Health = 130, MaxHealth = 130, Attack = 28, Defense = 9, ExperienceReward = 70, GoldReward = 65 },
            new Monster { Name = "Dragon", Emoji = "🐲", Health = 150, MaxHealth = 150, Attack = 30, Defense = 10, ExperienceReward = 100, GoldReward = 100 },
            new Monster { Name = "Ancient Dragon", Emoji = "🔥", Health = 200, MaxHealth = 200, Attack = 35, Defense = 12, ExperienceReward = 150, GoldReward = 150 }
        };

        public Monster CreateMonster(int dungeonLevel)
        {
            var maxMonsterTier = Math.Min((dungeonLevel + 2) / 3, monsterTemplates.Count);
            var minMonsterTier = Math.Max(0, maxMonsterTier - 3);

            var randomIndex = Random.Shared.Next(minMonsterTier, maxMonsterTier);
            var template = monsterTemplates[randomIndex];

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
}