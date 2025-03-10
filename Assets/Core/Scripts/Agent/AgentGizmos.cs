#if UNITY_EDITOR
using UnityEditor;
using SenteAI.Extra;
using UnityEngine;
using UnityEngine.AI;

namespace SenteAI.Core
{
    public class AgentGizmos : MonoBehaviour
    {
        private Agent _agent;
        private static readonly int TEXT_SIZE = 14;
        private static readonly float TEXT_HEIGHT = 4f;
        private static readonly float TEXT_SPACING = 0.2f;
        private static readonly Color PATH_COLOR = new(0f, 1f, 1f, 0.1f);

        private void Start()
        {
            _agent = gameObject.GetComponent<Agent>();
        }

        private void DrawActions()
        {
            GUIStyle style = new()
            {
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter,
                fontSize = TEXT_SIZE,
            };

            Vector3 textPosition = transform.position + Vector3.up * 2;

            foreach (var action in _agent.Actions)
            {
                textPosition += Vector3.down * TEXT_HEIGHT;

                string actionInfo =
                    $"{Helpers.CleanName(action.name)}={action.ScaledUtilityScore:F2}";

                style.normal.textColor = Color.white;
                Handles.Label(textPosition, actionInfo, style);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            // Draw main position indicator
            // Gizmos.color = Color.white;
            // Vector3 currentPos = _agent.Metrics.GetAveragePosition(6);
            // Gizmos.DrawWireCube(currentPos, Vector3.one * 5);
            // DrawLabel(currentPos, "Average(6)");

            if (_agent != null && _agent.Target != null)
            {
                var seeingModule = _agent.GetModule<SeeingModule>();
                Color targetColor =
                    seeingModule != null && seeingModule.HasLOS ? Color.green : Color.red;
                targetColor.a = 0.4f;
                Gizmos.color = targetColor;
                Gizmos.DrawLine(transform.position, _agent.Target.transform.position);
            }

            Vector3 basePosition = _agent.transform.position;
            float spacing = HandleUtility.GetHandleSize(basePosition) * TEXT_SPACING;

            DrawLabel(
                basePosition + Vector3.up * spacing * 2,
                Helpers.CleanName(_agent.GetModule<Brain>().CurrentAction?.name),
                Color.yellow
            );
            DrawLabel(
                basePosition + Vector3.up * spacing,
                _agent.Metrics.DistanceToTarget.ToString("F2"),
                Color.cyan
            );
            DrawLabel(basePosition, _agent.State.ToString(), Color.magenta);

            // Draw movement type indicator
            if (_agent.Metrics.IsClusteredMovement())
            {
                // Gizmos.color = Helpers.GetFactionColorHex(_agent.Faction);
                // Vector3 clusterPos = _agent.Metrics.GetAveragePosition(
                //     _agent.Metrics.recentHistorySize
                // );
                // Gizmos.DrawWireSphere(clusterPos, 2);
                // DrawLabel(clusterPos, "Clustered");
            }
            else
            {
                Gizmos.color = Helpers.GetFactionColorHex(_agent.Faction);
                Vector3 predictedPos = _agent.Metrics.GetPredictedPosition();
                Vector3 currentPos = _agent.transform.position;

                // Draw prediction path
                Color pathColor = Helpers.GetFactionColorHex(_agent.Faction);
                pathColor.a = 0.4f; // Make semi-transparent
                Gizmos.color = pathColor;
                Gizmos.DrawLine(currentPos, predictedPos);

                // Draw prediction cube
                Gizmos.color = Helpers.GetFactionColorHex(_agent.Faction);
                Gizmos.DrawWireCube(predictedPos, Vector3.one * 3);
                //DrawLabel(predictedPos, "Predicted");
            }

            // Visualize position history with small cubes
            if (_agent.Metrics.PositionHistory.Count > 0)
            {
                Gizmos.color = Helpers.GetFactionColorHex(_agent.Faction);
                foreach (Vector3 pos in _agent.Metrics.PositionHistory)
                {
                    Gizmos.DrawCube(pos, Vector3.one * 0.15f);
                }
            }

            var navMeshAgentModule = _agent.GetModule<NavMeshAgentModule>();
            if (navMeshAgentModule == null || navMeshAgentModule.NavMeshAgent == null)
                return;

            NavMeshPath path = navMeshAgentModule.NavMeshAgent.path;

            // Draw path with gradient
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                float t = (float)i / (path.corners.Length - 1);
                Gizmos.color = Color.Lerp(PATH_COLOR, Color.white, t);
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);

                // Larger spheres for start/end, smaller for middle points
                float sphereSize = (i == 0 || i == path.corners.Length - 1) ? 0.4f : 0.2f;
                Gizmos.DrawSphere(path.corners[i], sphereSize);
            }
        }

        private void DrawLabel(Vector3 position, string text, Color color)
        {
            float scaledSpacing = HandleUtility.GetHandleSize(position) * TEXT_SPACING;

            GUIStyle style = new()
            {
                normal = { textColor = color },
                fontSize = TEXT_SIZE,
                alignment = TextAnchor.MiddleCenter,
            };

            Handles.Label(position + Vector3.up * scaledSpacing, text, style);
        }
    }
}
#endif
