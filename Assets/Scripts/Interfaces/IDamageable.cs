using UnityEngine;

public interface IDamageable
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
    void TakeDamage(int amount);
    void Die();
}
