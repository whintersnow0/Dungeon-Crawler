namespace BlazorDungeon.Models
{
    public class Potion : Item
    {
        public int HealAmount { get; set; }

        public Potion()
        {
            Type = ItemType.Potion;
            Emoji = "🧪";
        }
    }
}