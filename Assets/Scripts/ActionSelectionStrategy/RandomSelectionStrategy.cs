using UnityEngine;

[CreateAssetMenu(
    fileName = "RandomSelectionStrategy",
    menuName = "ActionSelection/RandomSelectionStrategy"
)]
public class RandomSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        int randomIndex = Random.Range(0, agent.actionUtilityManager.actions.Count);
        return agent.actionUtilityManager.actions[randomIndex];
    }
}
