using UnityEngine;

[CreateAssetMenu(fileName = "HealthModule", menuName = "Module/HealthModule")]
public class HealthModule : AgentModule, IDamageable
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    Agent _agent;

    public override void ExecuteLoop(Agent agent) { }

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
        Destroy(_agent.gameObject);
    }
}
