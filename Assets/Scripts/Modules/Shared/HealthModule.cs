using UnityEngine;

[CreateAssetMenu(fileName = "HealthModule", menuName = "Module/HealthModule")]
public class HealthModule : Module, IDamageable
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public bool IsAlive { get; private set; } = true;
    public float TimeAlive { get; private set; } = 0;
    private Agent _agent;

    public override void Execute(Agent agent)
    {
        if (IsAlive)
            TimeAlive += Time.deltaTime;
    }

    public override void Initialize(Agent agent)
    {
        _agent = agent;
        MaxHealth = agent.Data.maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        IsAlive = false;
        Destroy(_agent.gameObject);
    }
}
