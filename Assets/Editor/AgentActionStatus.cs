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
        if (!Application.isPlaying)
            EditorGUILayout.LabelField(
                "Data displayed by an editor script (AgentActionStatus.cs)",
                EditorStyles.boldLabel
            );

        AgentUtilityManager manager = (AgentUtilityManager)target;

        foreach (var action in manager.actions)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(action.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Base Utility Score", action.baseUtility.ToString("F2"));
            EditorGUILayout.LabelField("Utility Score", action.utilityScore.ToString("F2"));
            EditorGUILayout.LabelField("Decay Factor", action.DecayFactor.ToString("F2"));
            EditorGUILayout.LabelField(
                "Decay Per Execution",
                action.decayPerExecution.ToString("F2")
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
