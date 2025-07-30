namespace BlazorDungeon.Models
{
    public class Weapon : Equipment
    {
        public Weapon()
        {
            Type = ItemType.Weapon;
            Emoji = "⚔️";
        }
    }
}