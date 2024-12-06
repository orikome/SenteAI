using UnityEngine;

[CreateAssetMenu(
    fileName = "SingleActionSelectionStrategy",
    menuName = "ActionSelection/SingleAction"
)]
public class SingleActionSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        if (agent.Actions.Count == 1)
        {
            return agent.Actions[0];
        }
        else
        {
            return null;
        }
    }
}
