using BlazorDungeon.Models;

namespace BlazorDungeon.Services
{
    public interface IItemFactory
    {
        List<Weapon> GetWeaponTemplates();
        List<Armor> GetArmorTemplates();
        List<Potion> GetPotionTemplates();
        Item CreateRandomItem();
    }
}