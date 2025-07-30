using BlazorDungeon.Models;

namespace BlazorDungeon.Services
{
    public interface IMonsterFactory
    {
        Monster CreateMonster(int dungeonLevel);
        List<Monster> GetMonsterTemplates();
    }
}
