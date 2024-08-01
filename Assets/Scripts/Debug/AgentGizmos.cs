#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AgentGizmos : MonoBehaviour
{
    AgentActionWeightManager actionDictionary;
    Agent agent;
    public float textHeight = 4f;
    public float textSize = 0.1f;

    private void Start()
    {
        actionDictionary = GetComponent<AgentActionWeightManager>();
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

        if (actionDictionary != null)
        {
            foreach (var actionProbability in actionDictionary.weights)
            {
                textPosition += Vector3.down * textHeight;

                string actionInfo =
                    $"A: {actionProbability.Key.name}, W: {actionProbability.Value:F2}, C: {actionProbability.Key.cost}";

                style.normal.textColor = Color.white;
                Handles.Label(textPosition, actionInfo, style);
            }
        }
        else
        {
            textPosition += Vector3.down * textHeight;
            style.normal.textColor = Color.red;
            Handles.Label(textPosition, "No ActionDictionary.", style);
        }
    }
}
#endif
