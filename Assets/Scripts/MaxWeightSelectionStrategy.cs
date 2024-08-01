using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MaxWeightSelectionStrategy",
    menuName = "ActionSelection/MaxWeightSelection"
)]
public class MaxWeightSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        return agent.actionWeightManager.weights.OrderByDescending(w => w.Value).First().Key;
    }
}
