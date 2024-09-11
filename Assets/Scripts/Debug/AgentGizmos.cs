using UnityEditor;
using UnityEngine;

public class AgentGizmos : MonoBehaviour
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
        if (!EditorApplication.isPlaying)
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
    }
}
