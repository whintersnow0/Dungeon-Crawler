namespace BlazorDungeon.Models
{
    public interface IInventoryHolder
    {
        List<Item> Inventory { get; set; }
        void AddItem(Item item);
        bool RemoveItem(Item item);
    }
}