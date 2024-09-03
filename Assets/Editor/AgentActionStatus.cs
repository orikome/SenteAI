using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AgentUtilityManager))]
public class AgentUtilityManagerInspector : Editor
{
    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    public override void OnInspectorGUI()
    {
        AgentUtilityManager manager = (AgentUtilityManager)target;

        //DrawDefaultInspector();
        //EditorGUILayout.Space();

        EditorGUILayout.LabelField("Action Status", EditorStyles.boldLabel);

        foreach (var action in manager.actions)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(action.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Utility Score", action.utilityScore.ToString("F2"));
            EditorGUILayout.LabelField(
                "Cooldown Time Remaining",
                action.GetCooldownTimeRemaining().ToString("F2") + " seconds"
            );
            EditorGUILayout.LabelField(
                "Cooldown Progress",
                (action.GetCooldownProgress() * 100).ToString("F0") + "%"
            );

            if (action.IsOnCooldown())
            {
                EditorGUILayout.HelpBox("This action is currently on cooldown.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
