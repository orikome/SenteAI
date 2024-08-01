using UnityEngine;

//using UnityEngine.AI;

[CreateAssetMenu(fileName = "MoveAction", menuName = "AgentAction/MoveAction")]
public class MoveAction : AgentAction
{
    [SerializeField]
    private float evaluationFrequency = 1f;

    //[SerializeField]
    //private float moveRadius = 10f;

    //[SerializeField]
    //private int samples = 10;
    private float lastEvaluationTime = 0f;

    public override void ExecuteAction(Transform firePoint, Agent agent)
    {
        if (Time.time - lastEvaluationTime > evaluationFrequency)
        {
            lastEvaluationTime = Time.time;
            Vector3 bestPosition = EvaluateBestPosition(agent);
            agent.SetDestination(bestPosition);

            // Increase weight if the position is good
            if (IsPositionGood(bestPosition))
            {
                agent.actionWeightManager.AdjustWeight(this, 0.1f);
            }
        }
    }

    private Vector3 EvaluateBestPosition(Agent agent)
    {
        //Vector3 bestPosition = Vector3.zero;
        //float bestScore = float.MinValue;

        //for (int i = 0; i < samples; i++)
        //{
        //    Vector3 randomPoint = agent.transform.position + Random.insideUnitSphere * moveRadius;
        //    NavMeshHit hit;
        //    if (NavMesh.SamplePosition(randomPoint, out hit, moveRadius, NavMesh.AllAreas))
        //    {
        //        Vector3 samplePosition = hit.position;
        //        float score = EvaluatePosition(samplePosition, agent);

        //        if (score > bestScore)
        //        {
        //            bestScore = score;
        //            bestPosition = samplePosition;
        //        }
        //    }
        //}

        //return bestPosition;

        return PlayerMovement.Instance.transform.position;
    }

    private float EvaluatePosition(Vector3 position, Agent agent)
    {
        // TODO: checking distance to player, cover, lines of sight, etc.
        // Use a scoring system to eval pos

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
