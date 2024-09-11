using UnityEngine;

public abstract class ActionSelectionStrategy : ScriptableObject
{
    public abstract AgentAction SelectAction(EnemyAgent agent);
}
