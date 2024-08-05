using UnityEngine;

public abstract class AgentModule : ScriptableObject
{
    public abstract void Execute(Agent agent);

    public abstract void RegisterEvents(AgentEvents agentEvents);
}
