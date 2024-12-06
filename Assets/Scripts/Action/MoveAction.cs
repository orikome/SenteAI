using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MoveAction", menuName = "AgentAction/MoveAction")]
public class MoveAction : AgentAction
{
    private readonly float _moveRadius = 20f;
    private readonly int _samples = 10;
    private Agent _enemy;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _enemy = agent;
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        PlayerMetrics playerMetrics = (PlayerMetrics)GameManager.Instance.playerAgent.Metrics;
        Vector3 predictedPlayerPosition = playerMetrics.PredictPosition();

        Vector3 bestPosition = EvaluateBestPosition(_enemy, predictedPlayerPosition);

        _enemy.GetModule<NavMeshAgentModule>().SetDestination(bestPosition);
        AfterExecution();
    }

    private Vector3 EvaluateBestPosition(Agent agent, Vector3 predictedPlayerPosition)
    {
        Vector3 bestPosition = Vector3.zero;
        float bestScore = float.MinValue;

        Vector3 sampleCenter;

        switch (GameManager.Instance.playerAgent.Metrics.CurrentBehavior)
        {
            case Behavior.Aggressive:
                // When the player is aggressive, sample positions around the agent to find cover
                sampleCenter = agent.transform.position;
                break;
            case Behavior.Defensive:
                // When the player is defensive, sample positions around the player to approach them
                sampleCenter = predictedPlayerPosition;
                break;
            case Behavior.Balanced:
                // Sample positions between the agent and the player
                sampleCenter = Vector3.Lerp(
                    agent.transform.position,
                    predictedPlayerPosition,
                    0.5f
                );
                break;
            default:
                sampleCenter = agent.transform.position;
                break;
        }

        for (int i = 0; i < _samples; i++)
        {
            Vector3 randomPoint = sampleCenter + Random.insideUnitSphere * _moveRadius;
            if (
                NavMesh.SamplePosition(
                    randomPoint,
                    out NavMeshHit hit,
                    _moveRadius,
                    NavMesh.AllAreas
                )
            )
            {
                Vector3 samplePosition = hit.position;
                float score = ScorePosition(samplePosition, agent, predictedPlayerPosition);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = samplePosition;
                }
            }
        }

        return bestPosition;
    }

    private float ScorePosition(Vector3 position, Agent agent, Vector3 predictedPlayerPosition)
    {
        float score = 0f;

        float distanceToPredictedPlayer = Vector3.Distance(position, predictedPlayerPosition);

        bool positionInCover = IsInCover(position);

        switch (GameManager.Instance.playerAgent.Metrics.CurrentBehavior)
        {
            case Behavior.Aggressive:
                // Player is aggressive; enemy should seek cover
                if (positionInCover)
                    score += 20f; // Encourage positions in cover
                else
                    score -= 10f; // Discourage positions not in cover

                // Maintain a safe distance
                score += Mathf.Clamp(distanceToPredictedPlayer, 10f, 30f);
                break;

            case Behavior.Defensive:
                // Player is defensive; enemy should approach
                score -= distanceToPredictedPlayer; // Encourage getting closer

                // Slightly prefer positions in cover
                if (positionInCover)
                    score += 5f;
                break;

            case Behavior.Balanced:
                // Neutral behavior
                score -= Mathf.Abs(distanceToPredictedPlayer - 15f); // Prefer moderate distance

                // Slight preference for cover
                if (positionInCover)
                    score += 10f;
                break;
        }

        // Check for line of sight to the player
        if (HasLineOfSight(position, predictedPlayerPosition))
        {
            score += 5f; // Bonus for visibility
        }
        else
        {
            score -= 5f; // Penalty for lack of visibility
        }

        return score;
    }

    public override void CalculateUtility(Agent agent)
    {
        float maxDistance = 100f;
        float canSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? MIN_UTILITY : 0.8f;

        Metrics metrics = agent.Metrics;
        float distance = metrics.DistanceToTarget;
        float distanceFactor = 1.0f - distance / maxDistance;
        float calculatedUtil = distanceFactor * canSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }

    private bool HasLineOfSight(Vector3 fromPosition, Vector3 targetPosition)
    {
        if (Physics.Raycast(fromPosition, targetPosition - fromPosition, out RaycastHit hit))
            return hit.transform == GameManager.Instance.playerAgent.transform;

        return false;
    }

    private bool IsInCover(Vector3 position)
    {
        Vector3 directionToPlayer = GameManager.Instance.playerAgent.transform.position - position;
        if (
            Physics.Raycast(
                position,
                directionToPlayer.normalized,
                out RaycastHit hit,
                directionToPlayer.magnitude
            )
        )
        {
            if (hit.transform != GameManager.Instance.playerAgent.transform)
            {
                return true; // There is an obstacle between position and player
            }
        }
        return false; // No obstacle, position is not in cover
    }
}
