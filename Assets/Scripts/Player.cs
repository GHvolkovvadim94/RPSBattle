using UnityEngine;

public class Player : ICombatant
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public Player(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        Debug.Log($"Player takes {amount} damage. Current health: {CurrentHealth}");
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        Debug.Log($"Player heals {amount}. Current health: {CurrentHealth}");
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth; // Восстанавливаем текущее здоровье до максимального
    }
}
