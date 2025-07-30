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
}