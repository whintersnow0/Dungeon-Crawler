namespace BlazorDungeon.Models
{
    public interface IEquippable
    {
        bool CanEquip(Item item);
        void EquipItem(Item item);
        void UnequipItem(ItemType itemType);
    }
}