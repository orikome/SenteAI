using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MoveAction", menuName = "AgentAction/MoveAction")]
public class MoveAction : AgentAction
{
    [SerializeField]
    private float moveRadius = 10f;

    [SerializeField]
    private int samples = 10;

    public override void ExecuteAction(Transform firePoint, Agent agent)
    {
        Vector3 bestPosition = EvaluateBestPosition(agent);
        agent.SetDestination(bestPosition);

        if (IsPositionGood(bestPosition))
            agent.actionWeightManager.AdjustWeight(this, 0.1f);
    }

    private Vector3 EvaluateBestPosition(Agent agent)
    {
        // Vector3 bestPosition = Vector3.zero;
        // float bestScore = float.MinValue;

        // // TODO: checking distance to player, cover, lines of sight, etc.
        // // Use a scoring system to eval pos
        // // If agent some state, prefer ranged, melee or seek cover?

        // for (int i = 0; i < samples; i++)
        // {
        //     Vector3 randomPoint = agent.transform.position + Random.insideUnitSphere * moveRadius;
        //     NavMeshHit hit;
        //     if (NavMesh.SamplePosition(randomPoint, out hit, moveRadius, NavMesh.AllAreas))
        //     {
        //         Vector3 samplePosition = hit.position;
        //         float score = ScorePosition(samplePosition, agent);

        //         if (score > bestScore)
        //         {
        //             bestScore = score;
        //             bestPosition = samplePosition;
        //         }
        //     }
        // }

        // return bestPosition;

        return Player.Instance.transform.position;
    }

    private float ScorePosition(Vector3 position, Agent agent)
    {
        float score = 0f;

        // Prefer positions closer to the player
        float distanceToPlayer = Vector3.Distance(position, agent.transform.position);
        score -= distanceToPlayer;

        return score;
    }

    private bool IsPositionGood(Vector3 position)
    {
        return true;
    }
}
