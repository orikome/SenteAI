using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    [Range(0, 100)]
    public int cost;
    public abstract void Initialize(Agent agent);
    public abstract void ExecuteAction(Transform firePoint, Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public virtual void UpdateWeights(Agent agent) { }
}
