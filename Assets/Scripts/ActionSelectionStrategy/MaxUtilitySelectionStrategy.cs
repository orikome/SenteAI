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
        return agent
            .ActionUtilityManager.actions.Where(action => action.CanExecute(agent))
            .OrderByDescending(action => action.utilityScore)
            .FirstOrDefault();
    }
}
