using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    [Range(0, 100)]
    public int cost;

    [Range(0.0f, 1.0f)]
    public float utilityScore;
    public abstract void Initialize(Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public abstract void ExecuteActionLoop(Transform firePoint, Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public virtual void UpdateUtilityLoop(Agent agent) { }
}
