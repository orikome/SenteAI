using UnityEngine;

namespace SenteAI.Core
{
    public static class AgentLogger
    {
        private static string FRAME_COUNT => $"[{Time.frameCount}]";

        public static void Log(string message, GameObject go, GameObject targetGo)
        {
            string firstPrefix = $"[{go.GetInstanceID()}]";
            string secondPrefix = $"[{targetGo.GetInstanceID()}]";
            string coloredFirstPrefix = GetColoredPrefix(go, firstPrefix);
            string coloredSecondPrefix = GetColoredPrefix(targetGo, secondPrefix);

            message = $"{FRAME_COUNT} {coloredFirstPrefix}->{coloredSecondPrefix} {message}";
            Debug.Log(message);
        }

        public static void Log(string message, GameObject go)
        {
            string prefix = $"[{go.GetInstanceID()}]";
            string coloredPrefix = GetColoredPrefix(go, prefix);

            message = $"{FRAME_COUNT} {coloredPrefix} {message}";
            Debug.Log(message);
        }

        public static void Log(string message)
        {
            message = $"{FRAME_COUNT} {message}";
            Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            string warningPrefix = Helpers.Color(Helpers.Bold("[WARNING]"), Color.yellow);
            message = $"{FRAME_COUNT} {warningPrefix} {message}";
            Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            string errorPrefix = Helpers.Color(Helpers.Bold("[ERROR]"), Color.red);
            message = $"{FRAME_COUNT} {errorPrefix} {message}";
            Debug.LogError(message);
        }

        private static string GetColoredPrefix(GameObject go, string prefix)
        {
            if (go.CompareTag("Player"))
                return Helpers.Color(prefix, Helpers.GetFactionColorHex(Faction.Player));
            if (go.CompareTag("Enemy"))
                return Helpers.Color(prefix, Helpers.GetFactionColorHex(Faction.Enemy));
            if (go.CompareTag("Ally"))
                return Helpers.Color(prefix, Helpers.GetFactionColorHex(Faction.Ally));
            return prefix;
        }
    }
}
