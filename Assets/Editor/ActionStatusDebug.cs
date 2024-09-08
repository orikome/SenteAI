using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Agent))]
public class ActionStatusDebug : Editor
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
        DrawDefaultInspector();

        if (!Application.isPlaying)
            return;

        Agent agent = (Agent)target;

        foreach (var action in agent.Actions)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(action.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Base Utility Score", action.baseUtility.ToString("F2"));
            EditorGUILayout.LabelField("Utility Score", action.utilityScore.ToString("F2"));
            EditorGUILayout.LabelField("Penalty Factor", action.PenaltyFactor.ToString("F2"));
            EditorGUILayout.LabelField(
                "Penalty Per Execution",
                action.penaltyPerExecution.ToString("F2")
            );
            EditorGUILayout.LabelField("Cooldown", action.cooldownTime.ToString("F2"));

            float cooldownProgress = action.GetCooldownProgress();
            float cooldownTimeRemaining = action.GetCooldownTimeRemaining();

            EditorGUILayout.LabelField("Cooldown Progress");

            EditorGUILayout.BeginHorizontal();
            GUILayout.HorizontalSlider(cooldownProgress, 0f, 1f);
            EditorGUILayout.LabelField(
                (cooldownProgress * 100).ToString("F0") + "%",
                GUILayout.Width(50)
            );
            EditorGUILayout.LabelField(
                cooldownTimeRemaining.ToString("F1") + "s",
                GUILayout.Width(50)
            );
            EditorGUILayout.EndHorizontal();

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