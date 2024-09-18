using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerInputSelectionStrategy",
    menuName = "ActionSelection/PlayerInputSelection"
)]
public class PlayerInputSelectionStrategy : ActionSelectionStrategy
{
    public KeyCode selectionKey = KeyCode.Mouse0;

    public override AgentAction SelectAction(Agent agent)
    {
        if (Player.Instance.IsInputHeld())
            return agent.Actions.FirstOrDefault();

        return null;
    }

    public override Vector3 GetShootDirection(Agent agent)
    {
        PlayerMetrics playerMetrics = (PlayerMetrics)Player.Instance.Metrics;
        var nearestEnemy = playerMetrics.FindClosestEnemyToPlayer();
        if (nearestEnemy != null)
        {
            return (nearestEnemy.position - agent.firePoint.position).normalized;
        }

        return agent.firePoint.forward;
    }
}
