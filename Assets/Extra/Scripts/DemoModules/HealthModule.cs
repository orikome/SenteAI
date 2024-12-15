using UnityEngine;

[CreateAssetMenu(fileName = "HealthModule", menuName = "SenteAI/Modules/HealthModule")]
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
        DebugManager.Instance.SpawnTextLog(_agent.transform, amount.ToString(), Color.white);

        _agent.Metrics.UpdateDamageTaken(amount);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        IsAlive = false;
        if (_agent.gameObject.CompareTag("Player"))
            AgentManager.Instance.RestartScene();
        else
            Destroy(_agent.gameObject);
    }
}
