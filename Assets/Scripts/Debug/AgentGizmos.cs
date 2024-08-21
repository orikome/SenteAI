#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AgentGizmos : MonoBehaviour
{
    AgentActionUtilityManager actionUtilityManager;
    Agent agent;
    public float textHeight = 4f;
    public float textSize = 0.1f;

    private void Start()
    {
        actionUtilityManager = GetComponent<AgentActionUtilityManager>();
        agent = GetComponent<Agent>();
    }

    private void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying)
            return;

        GUIStyle style = new GUIStyle
        {
            normal = { textColor = Color.yellow },
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20 // Adjust as needed
        };

        Vector3 textPosition = transform.position + Vector3.up * 2;

        // Display energy
        //string energyText = $"E: {agent.actionDecisionMaker.curEnergy:F0}";
        //Handles.Label(textPosition, energyText, style);

        if (
            agent != null
            && agent.actionUtilityManager != null
            && agent.actionUtilityManager.actions != null
        )
        {
            foreach (var action in agent.actionUtilityManager.actions)
            {
                textPosition += Vector3.down * textHeight;

                string actionInfo =
                    $"A: {action.name}, W: {action.utilityScore:F2}, C: {action.cost}";

                style.normal.textColor = Color.white;
                Handles.Label(textPosition, actionInfo, style);
            }
        }
        else
        {
            textPosition += Vector3.down * textHeight;
            style.normal.textColor = Color.red;
            Handles.Label(textPosition, "No Actions Found.", style);
        }
    }
}
#endif
