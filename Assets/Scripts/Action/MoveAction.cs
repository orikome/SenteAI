using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MoveAction", menuName = "AgentAction/MoveAction")]
public class MoveAction : AgentAction
{
    [SerializeField]
    private float moveRadius = 10f;

    [SerializeField]
    private int _samples = 10;

    private ActionReadinessModule _actionReadinessModule;

    public override void Initialize(Agent agent)
    {
        _actionReadinessModule = agent.GetModule<ActionReadinessModule>();
        Debug.Assert(_actionReadinessModule != null, "ActionReadinessModule is not set!");
    }

    public override void ExecuteLoop(Transform firePoint, Agent agent)
    {
        //Vector3 bestPosition = EvaluateBestPosition(agent);
        Vector3 bestPosition = Player.Instance.PlayerMetrics.PredictPositionDynamically();
        //Vector3 bestPosition = Player.Instance.transform.position;
        agent.SetDestination(bestPosition);

        lastExecutedTime = Time.time;
    }

    public override bool CanExecute(Agent agent)
    {
        return GetCooldownTimeRemaining() <= 0;
    }

    private Vector3 EvaluateBestPosition(Agent agent)
    {
        Vector3 bestPosition = Vector3.zero;
        float bestScore = float.MinValue;

        // TODO: checking distance to player, cover, lines of sight, etc.
        // Use a scoring system to eval pos
        // If agent some state, prefer ranged, melee or seek cover?

        for (int i = 0; i < _samples; i++)
        {
            Vector3 randomPoint = agent.transform.position + Random.insideUnitSphere * moveRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, moveRadius, NavMesh.AllAreas))
            {
                Vector3 samplePosition = hit.position;
                float score = ScorePosition(samplePosition, agent);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = samplePosition;
                }
            }
        }

        return bestPosition;
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
