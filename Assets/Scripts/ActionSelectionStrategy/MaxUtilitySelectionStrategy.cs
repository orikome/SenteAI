using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MaxUtilitySelectionStrategy",
    menuName = "ActionSelection/MaxUtilitySelection"
)]
public class MaxUtilitySelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        AgentAction selectedAction = agent
            .Actions.Where(action => action.CanExecute(agent))
            .OrderByDescending(action => action.ScaledUtilityScore)
            .FirstOrDefault();

        return selectedAction;
    }
}
