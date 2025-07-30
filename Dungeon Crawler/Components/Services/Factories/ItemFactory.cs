using BlazorDungeon.Models;

namespace BlazorDungeon.Services
{
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
}