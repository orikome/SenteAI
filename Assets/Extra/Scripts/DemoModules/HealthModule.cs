using UnityEngine;

[CreateAssetMenu(fileName = "HealthModule", menuName = "SenteAI/Modules/HealthModule")]
public class HealthModule : Module, IDamageable
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public bool IsAlive { get; private set; } = true;
    public float TimeAlive { get; private set; } = 0;

    public override void Execute()
    {
        if (IsAlive)
            TimeAlive += Time.deltaTime;
    }

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        MaxHealth = agent.data.maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (_agent.Faction == Faction.Player)
            CanvasManager.Instance.ShowDamageFlash();

        _agent.GetComponent<AgentExtra>().TriggerFlash();

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
