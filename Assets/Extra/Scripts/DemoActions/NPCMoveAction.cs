using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NPCMoveAction", menuName = "SenteAI/Actions/NPCMoveAction")]
public class NPCMoveAction : AgentAction
{
    private readonly float _moveRadius = 20f;
    private readonly int _samples = 10;

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        Metrics metrics = _agent.Metrics;
        Vector3 predictedTargetPosition = metrics.PredictPosition();

        Vector3 bestPosition = EvaluateBestPosition(_agent, predictedTargetPosition);

        _agent.GetModule<NavMeshAgentModule>().SetDestination(bestPosition);
        AfterExecution();
    }

    private Vector3 EvaluateBestPosition(Agent agent, Vector3 predictedTargetPosition)
    {
        Vector3 bestPosition = Vector3.zero;
        float bestScore = float.MinValue;

        Vector3 sampleCenter;

        switch (_agent.Target.Metrics.CurrentBehavior)
        {
            case Behavior.Aggressive:
                // When the target is aggressive, sample positions around the agent to find cover
                sampleCenter = agent.transform.position;
                break;
            case Behavior.Defensive:
                // When the target is defensive, sample positions around the player to approach them
                sampleCenter = predictedTargetPosition;
                break;
            case Behavior.Balanced:
                // Sample positions between the agent and the target
                sampleCenter = Vector3.Lerp(
                    agent.transform.position,
                    predictedTargetPosition,
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
                float score = ScorePosition(samplePosition, agent, predictedTargetPosition);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = samplePosition;
                }
            }
        }

        return bestPosition;
    }

    private float ScorePosition(Vector3 position, Agent agent, Vector3 predictedTargetPosition)
    {
        float score = 0f;

        float distanceToPredictedTargetPosition = Vector3.Distance(
            position,
            predictedTargetPosition
        );

        bool positionInCover = IsInCover(position);

        switch (_agent.Target.Metrics.CurrentBehavior)
        {
            case Behavior.Aggressive:
                // Target is aggressive; enemy should seek cover
                if (positionInCover)
                    score += 20f; // Encourage positions in cover
                else
                    score -= 10f; // Discourage positions not in cover

                // Maintain a safe distance
                score += Mathf.Clamp(distanceToPredictedTargetPosition, 10f, 30f);
                break;

            case Behavior.Defensive:
                // Target is defensive; enemy should approach
                score -= distanceToPredictedTargetPosition; // Encourage getting closer

                // Slightly prefer positions in cover
                if (positionInCover)
                    score += 5f;
                break;

            case Behavior.Balanced:
                // Neutral behavior
                score -= Mathf.Abs(distanceToPredictedTargetPosition - 15f); // Prefer moderate distance

                // Slight preference for cover
                if (positionInCover)
                    score += 10f;
                break;
        }

        // Check for line of sight to the player
        if (HasLineOfSight(position, predictedTargetPosition))
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
            return hit.transform == _agent.Target.transform;

        return false;
    }

    private bool IsInCover(Vector3 position)
    {
        Vector3 directionToTarget = _agent.Target.transform.position - position;
        if (
            Physics.Raycast(
                position,
                directionToTarget.normalized,
                out RaycastHit hit,
                directionToTarget.magnitude
            )
        )
        {
            if (hit.transform != _agent.Target.transform)
            {
                return true; // There is an obstacle between position and target
            }
        }
        return false; // No obstacle, position is not in cover
    }
}
