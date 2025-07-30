namespace BlazorDungeon.Models
{
    public class Armor : Equipment
    {
        public Armor()
        {
            Type = ItemType.Armor;
            Emoji = "🛡️";
        }
    }
}