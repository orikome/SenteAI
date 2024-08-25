using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

[CreateAssetMenu(fileName = "MoveAction", menuName = "AgentAction/MoveAction")]
public class MoveAction : AgentAction
{
    private readonly float _moveRadius = 20f;
    private readonly int _samples = 10;
    private ActionReadinessModule _actionReadinessModule;

    public override bool CanExecute(Agent agent)
    {
        return GetCooldownTimeRemaining() <= 0;
    }

    public override void Initialize(Agent agent)
    {
        _actionReadinessModule = agent.GetModule<ActionReadinessModule>();
        Debug.Assert(_actionReadinessModule != null, "ActionReadinessModule is not set!");
    }

    public override void ExecuteLoop(Transform firePoint, Agent agent)
    {
        Vector3 predictedPlayerPosition =
            Player.Instance.PlayerMetrics.PredictPositionDynamically();

        Vector3 bestPosition = EvaluateBestPosition(agent, predictedPlayerPosition);

        agent.SetDestination(bestPosition);

        lastExecutedTime = Time.time;
        CalculateUtility(agent, agent.AgentMetrics);
    }

    private Vector3 EvaluateBestPosition(Agent agent, Vector3 predictedPlayerPosition)
    {
        Vector3 bestPosition = Vector3.zero;
        float bestScore = float.MinValue;

        for (int i = 0; i < _samples; i++)
        {
            Vector3 randomPoint = agent.transform.position + Random.insideUnitSphere * _moveRadius;
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

        switch (Player.Instance.PlayerMetrics.currentBehavior)
        {
            case Behavior.Aggressive:
                score -= Mathf.Clamp(distanceToPredictedPlayer, 0, 15);
                break;

            case Behavior.Defensive:
                score += Mathf.Clamp(distanceToPredictedPlayer, 10, 30);
                break;

            case Behavior.Balanced:
                score += Mathf.Clamp(distanceToPredictedPlayer, 5, 20);
                break;
        }

        if (Player.Instance.PlayerMetrics.IsInCover)
        {
            if (HasLineOfSight(position, predictedPlayerPosition))
            {
                score += 10f;
            }
            else
            {
                score -= 5f;
            }
        }

        return score;
    }

    public override float CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        if (!agent.PerceptionModule.CanSenseTarget)
            return 1.0f;

        float distance = agent.AgentMetrics.DistanceToPlayer;

        return Mathf.Clamp01(1.0f - distance / 100f)
            * Mathf.Clamp01(agent.AgentMetrics.HealthFactor)
            * Mathf.Clamp01(0.5f);
    }

    private bool HasLineOfSight(Vector3 fromPosition, Vector3 targetPosition)
    {
        if (Physics.Raycast(fromPosition, targetPosition - fromPosition, out RaycastHit hit))
            return hit.transform == Player.Instance.transform;

        return false;
    }
}
