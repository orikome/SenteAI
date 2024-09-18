using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGizmos : MonoBehaviour
{
    private Enemy _agent;
    public float textHeight = 4f;
    public float textSize = 0.1f;

    private void Start()
    {
        _agent = gameObject.GetComponent<Enemy>();
    }

    private void OnDrawGizmos()
    {
        if (
            !EditorApplication.isPlaying
            || _agent.GetModule<NavMeshAgentModule>().NavMeshAgent == null
        )
            return;

        GUIStyle style =
            new()
            {
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
            };

        Vector3 textPosition = transform.position + Vector3.up * 2;

        foreach (var action in _agent.Actions)
        {
            textPosition += Vector3.down * textHeight;

            string actionInfo = $"{Helpers.CleanName(action.name)}={action.ScaledUtilityScore:F2}";

            style.normal.textColor = Color.white;
            Handles.Label(textPosition, actionInfo, style);
        }

        Gizmos.color = Color.cyan;

        NavMeshPath path = _agent.GetModule<NavMeshAgentModule>().NavMeshAgent.path;

        // Draw path
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
        }

        // Draw spheres at path corners
        Gizmos.color = Color.red;
        foreach (Vector3 corner in path.corners)
        {
            Gizmos.DrawSphere(corner, 0.2f);
        }
    }
}
