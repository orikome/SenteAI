using SenteAI.Core;
using UnityEditor;
using UnityEngine;

namespace SenteAI.Extra
{
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

            if (!AgentManager.Instance)
                return;

            Agent agent = (Agent)target;
            EditorGUILayout.LabelField(
                "Health",
                agent.GetModule<HealthModule>()?.CurrentHealth.ToString()
            );
            EditorGUILayout.LabelField(
                "TimeAlive",
                agent.GetModule<HealthModule>()?.TimeAlive.ToString()
            );

            EditorGUILayout.LabelField("DodgeRatio", agent.Metrics?.DodgeRatio.ToString("F2"));

            EditorGUILayout.LabelField("DamageDone", agent.Metrics?.DamageDone.ToString());
            EditorGUILayout.LabelField("DamageTaken", agent.Metrics?.DamageTaken.ToString());

            EditorGUILayout.LabelField("Target", agent.Target?.ToString());
            EditorGUILayout.LabelField(
                "DistanceToTarget",
                agent.Metrics?.DistanceToTarget.ToString()
            );

            float totalAPM = 0f;

            foreach (var action in agent.Actions)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField(action.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Times Executed", action.TimesExecuted.ToString());

                // Used Per Minute
                float timeAliveInMinutes = agent.GetModule<HealthModule>().TimeAlive / 60f;
                int actionUsedPerMinute = Mathf.RoundToInt(
                    action.TimesExecuted / timeAliveInMinutes
                );
                EditorGUILayout.LabelField("Used Per Minute", actionUsedPerMinute.ToString());

                EditorGUILayout.LabelField("Bias Weight", action.biasWeight.ToString("F2"));
                EditorGUILayout.LabelField(
                    "Unscaled Utility Score",
                    action.UnscaledUtilityScore.ToString("F2")
                );
                EditorGUILayout.LabelField(
                    "Scaled Utility Score",
                    action.ScaledUtilityScore.ToString("F2")
                );
                EditorGUILayout.LabelField("Penalty Factor", action.PenaltyFactor.ToString("F2"));
                EditorGUILayout.LabelField(
                    "Penalty Per Execution",
                    action.penaltyPerExecution.ToString("F2")
                );
                if (action is IFeedbackAction feedbackAction)
                    EditorGUILayout.LabelField(
                        "Feedback Modifier",
                        feedbackAction.FeedbackModifier.ToString("F2")
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
                    EditorGUILayout.HelpBox(
                        "This action is currently on cooldown.",
                        MessageType.Info
                    );
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                int actionAPM = Mathf.RoundToInt(action.TimesExecuted / timeAliveInMinutes);
                totalAPM += actionAPM;
            }

            EditorGUILayout.LabelField("APM", totalAPM.ToString());

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
