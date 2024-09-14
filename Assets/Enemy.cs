using UnityEngine;

public class Enemy : ICombatant
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public Enemy(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        Debug.Log($"Enemy takes {amount} damage. Current health: {CurrentHealth}");
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        Debug.Log($"Enemy heals {amount}. Current health: {CurrentHealth}");
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth; // Восстанавливаем текущее здоровье до максимального
    }
}
