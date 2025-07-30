using BlazorDungeon.Models;

namespace BlazorDungeon.Services
{
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
}