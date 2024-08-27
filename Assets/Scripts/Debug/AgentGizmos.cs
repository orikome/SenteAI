using UnityEditor;
using UnityEngine;

public class AgentGizmos : MonoBehaviour
{
    public Agent agent;
    public float textHeight = 4f;
    public float textSize = 0.1f;

    private void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying)
            return;

        GUIStyle style =
            new()
            {
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20
            };

        Vector3 textPosition = transform.position + Vector3.up * 2;

        // Display energy
        //string energyText = $"E: {agent.actionDecisionMaker.curEnergy:F0}";
        //Handles.Label(textPosition, energyText, style);

        foreach (var action in agent.ActionUtilityManager.actions)
        {
            textPosition += Vector3.down * textHeight;

            string actionInfo = $"A: {action.name}, U: {action.utilityScore:F2}, C: {action.cost}";

            style.normal.textColor = Color.white;
            Handles.Label(textPosition, actionInfo, style);
        }
    }
}
