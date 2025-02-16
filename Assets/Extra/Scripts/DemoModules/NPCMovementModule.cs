using SenteAI.Core;
using UnityEngine;
using UnityEngine.AI;

namespace SenteAI.Extra
{
    [CreateAssetMenu(
        fileName = "NPCMovementModule",
        menuName = "SenteAI/Modules/NPCMovementModule"
    )]
    public class NPCMovementModule : Module
    {
        private const float MOVE_RADIUS = 30f;
        private const int SAMPLE_AMOUNT = 4;

        public override void Execute()
        {
            if (_agent.Target == null || Time.frameCount % 32 != 0)
                return;

            Vector3 targetPosition = _agent.Target.transform.position;
            Vector3 bestPosition = EvaluateBestPosition(_agent, targetPosition);
            _agent.GetModule<NavMeshAgentModule>().SetDestination(bestPosition);
        }

        private Vector3 EvaluateBestPosition(Agent agent, Vector3 targetPosition)
        {
            Vector3 bestPosition = Vector3.zero;
            float bestScore = float.MinValue;
            Vector3 sampleCenter = agent.transform.position;

            for (int i = 0; i < SAMPLE_AMOUNT; i++)
            {
                Vector3 randomPoint = sampleCenter + Random.insideUnitSphere * MOVE_RADIUS;
                if (
                    NavMesh.SamplePosition(
                        randomPoint,
                        out NavMeshHit hit,
                        MOVE_RADIUS,
                        NavMesh.AllAreas
                    )
                )
                {
                    Vector3 samplePosition = hit.position;
                    float score = ScorePosition(samplePosition, targetPosition);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPosition = samplePosition;
                    }
                }
            }

            return bestPosition;
        }

        private float ScorePosition(Vector3 position, Vector3 targetPosition)
        {
            float score = 0f;

            score += ScoreDistance(position, targetPosition);
            score += ScoreCover(position);
            score += ScoreLineOfSight(position, targetPosition);
            score += ScoreLastHitAngleAvoidance(position);

            return score;
        }

        private float ScoreDistance(Vector3 position, Vector3 targetPosition)
        {
            float distance = OrikomeUtils.GeneralUtils.GetDistanceSquared(position, targetPosition);
            return -distance;
        }

        private float ScoreCover(Vector3 position)
        {
            return !HasLineOfSight(position, _agent.Target.transform.position) ? 10f : 0f;
        }

        private float ScoreLineOfSight(Vector3 position, Vector3 targetPosition)
        {
            return HasLineOfSight(position, targetPosition) ? 5f : -5f;
        }

        private float ScoreLastHitAngleAvoidance(Vector3 position)
        {
            var healthModule = _agent.GetModule<HealthModule>();
            if (healthModule == null || healthModule.LastHitAngle == Vector3.zero)
                return 0f;

            // Calculate how exposed this position is to the last hit direction
            Vector3 directionFromHit = (position - _agent.transform.position).normalized;
            float angleScore = Vector3.Dot(directionFromHit, healthModule.LastHitAngle);

            // High positive dot product means the position is in the same direction as the hit
            return -angleScore * 15f;
        }

        private bool HasLineOfSight(Vector3 fromPosition, Vector3 targetPosition)
        {
            if (Physics.Raycast(fromPosition, targetPosition - fromPosition, out RaycastHit hit))
                return hit.transform == _agent.Target.transform;
            return false;
        }
    }
}
