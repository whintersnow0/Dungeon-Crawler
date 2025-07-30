namespace BlazorDungeon.Models
{
    public abstract class Item
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Value { get; set; }
        public string Emoji { get; set; } = "📦";
        public ItemType Type { get; protected set; }
    }
}