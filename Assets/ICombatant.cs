public interface ICombatant
{
    int CurrentHealth { get; }
    void TakeDamage(int amount);
    void Heal(int amount);
    void ResetHealth();
}
