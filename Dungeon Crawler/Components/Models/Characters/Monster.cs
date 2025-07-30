namespace BlazorDungeon.Models
{
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
}