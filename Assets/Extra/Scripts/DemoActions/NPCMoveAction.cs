using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(fileName = "NPCMoveAction", menuName = "SenteAI/Actions/NPCMoveAction")]
    public class NPCMoveAction : AgentAction
    {
        public override void Execute(Transform firePoint, Vector3 direction)
        {
            Metrics metrics = _agent.Metrics;
            Vector3 predictedTargetPosition = metrics.GetPredictedPosition();

            _agent.GetModule<NavMeshAgentModule>().SetDestination(predictedTargetPosition);
            AfterExecution();
        }

        public override void CalculateUtility()
        {
            float maxDistance = 100f;
            float canSenseFactor = _agent.GetModule<SenseModule>().CanSenseTarget
                ? MIN_UTILITY
                : 0.8f;

            Metrics metrics = _agent.Metrics;
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
}
