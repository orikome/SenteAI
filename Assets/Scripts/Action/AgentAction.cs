using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    [Range(0, 100)]
    public int cost;
    public abstract void ExecuteAction(Transform firePoint, Agent agent);

    public virtual void UpdateWeights(Agent agent) { }
}
