using UnityEngine;

public abstract class AgentModule : ScriptableObject
{
    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public abstract void ExecuteLoop(Agent agent);

    /// <summary>
    /// Called once in the agent's awake function.
    /// </summary>
    public abstract void Initialize(Agent agent);
}
