using UnityEngine;

public abstract class ActionSelectionStrategy : ScriptableObject
{
    public abstract AgentAction SelectAction(Agent agent);

    public virtual Vector3 GetShootDirection(Agent agent)
    {
        return agent.transform.forward;
    }
}
