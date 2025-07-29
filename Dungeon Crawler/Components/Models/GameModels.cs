namespace BlazorDungeon.Models
{
    public interface ICharacter
    {
        string Name { get; set; }
        int Health { get; set; }
        int MaxHealth { get; set; }
        int Attack { get; set; }
        int Defense { get; set; }
        bool IsAlive { get; }
        void TakeDamage(int damage);
    }

    public interface IInventoryHolder
    {
        List<Item> Inventory { get; set; }
        void AddItem(Item item);
        bool RemoveItem(Item item);
    }

    public interface IEquippable
    {
        bool CanEquip(Item item);
        void EquipItem(Item item);
        void UnequipItem(ItemType itemType);
    }

    public class Player : ICharacter, IInventoryHolder, IEquippable
    {
        public string Name { get; set; } = "Hero";
        public int Level { get; set; } = 1;
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int Attack { get; set; } = 15;
        public int Defense { get; set; } = 5;
        public int Experience { get; set; } = 0;
        public int ExperienceToNext => Level * 50;
        public int Gold { get; set; } = 50;
        public List<Item> Inventory { get; set; } = new();
        public Weapon? EquippedWeapon { get; set; }
        public Armor? EquippedArmor { get; set; }

        public int TotalAttack => Attack + (EquippedWeapon?.Bonus ?? 0);
        public int TotalDefense => Defense + (EquippedArmor?.Bonus ?? 0);
        public bool CanLevelUp => Experience >= ExperienceToNext;
        public bool IsAlive => Health > 0;

        public void LevelUp()
        {
            if (!CanLevelUp) return;

            Level++;
            Experience -= ExperienceToNext;

            var healthIncrease = Random.Shared.Next(8, 15);
            var attackIncrease = Random.Shared.Next(2, 5);
            var defenseIncrease = Random.Shared.Next(1, 3);

            MaxHealth += healthIncrease;
            Health = MaxHealth;
            Attack += attackIncrease;
            Defense += defenseIncrease;
        }

        public void TakeDamage(int damage)
        {
            var actualDamage = Math.Max(1, damage - TotalDefense);
            Health = Math.Max(0, Health - actualDamage);
        }

        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
        }

        public bool RemoveItem(Item item)
        {
            return Inventory.Remove(item);
        }

        public bool CanEquip(Item item)
        {
            return item is Weapon or Armor;
        }

        public void EquipItem(Item item)
        {
            switch (item)
            {
                case Weapon weapon:
                    EquippedWeapon = weapon;
                    break;
                case Armor armor:
                    EquippedArmor = armor;
                    break;
            }
        }

        public void UnequipItem(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    EquippedWeapon = null;
                    break;
                case ItemType.Armor:
                    EquippedArmor = null;
                    break;
            }
        }
    }

    public class Monster : ICharacter
    {
        public string Name { get; set; } = "";
        public string Emoji { get; set; } = "👹";
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int ExperienceReward { get; set; }
        public int GoldReward { get; set; }
        public List<Item> PossibleLoot { get; set; } = new();

        public bool IsAlive => Health > 0;

        public void TakeDamage(int damage)
        {
            var actualDamage = Math.Max(1, damage - Defense);
            Health = Math.Max(0, Health - actualDamage);
        }

        public Item? DropLoot()
        {
            if (PossibleLoot.Any() && Random.Shared.Next(100) < 30)
            {
                return PossibleLoot[Random.Shared.Next(PossibleLoot.Count)];
            }
            return null;
        }
    }

    public enum ItemType
    {
        Weapon,
        Armor,
        Potion
    }

    public abstract class Item
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Value { get; set; }
        public string Emoji { get; set; } = "📦";
        public ItemType Type { get; protected set; }
    }

    public abstract class Equipment : Item
    {
        public int Bonus { get; set; }
    }

    public class Weapon : Equipment
    {
        public Weapon()
        {
            Type = ItemType.Weapon;
            Emoji = "⚔️";
        }
    }

    public class Armor : Equipment
    {
        public Armor()
        {
            Type = ItemType.Armor;
            Emoji = "🛡️";
        }
    }

    public class Potion : Item
    {
        public int HealAmount { get; set; }

        public Potion()
        {
            Type = ItemType.Potion;
            Emoji = "🧪";
        }
    }

    public enum GameState
    {
        Menu,
        Exploring,
        Combat,
        Victory,
        GameOver,
        Inventory,
        Shop
    }
}