using System.Collections.Generic;

public class EnemyMetrics : Metrics
{
    public float DistanceToPlayer { get; private set; }
    public float HealthFactor { get; private set; }
    public float EnergyLevel { get; private set; }
    public List<AgentAction> ActionHistory { get; private set; } = new();

    void Update()
    {
        CurrentBehavior = ClassifyBehavior();
        UpdateVelocity();
    }

    public void SetDistanceToPlayer(float disToPlayer)
    {
        DistanceToPlayer = disToPlayer;
    }

    public void AddActionToHistory(AgentAction action)
    {
        ActionHistory.Add(action);
        if (ActionHistory.Count > 20)
        {
            ActionHistory.RemoveAt(0);
        }
    }
}
