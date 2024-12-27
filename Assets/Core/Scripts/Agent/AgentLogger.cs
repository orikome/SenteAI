using UnityEngine;

public static class AgentLogger
{
    private static string FRAME_COUNT => $"[Frame {Time.frameCount}]";

    public static void Log(string message, GameObject go, GameObject targetGo)
    {
        string firstPrefix = $"[{go.GetInstanceID()}]";
        string secondPrefix = $"[{targetGo.GetInstanceID()}]";
        string coloredFirstPrefix,
            coloredSecondPrefix;

        // Color owner GameObject
        if (go.CompareTag("Player"))
        {
            coloredFirstPrefix =
                $"<color={Helpers.GetFactionColor(Faction.Player)}>{firstPrefix}</color>";
        }
        else if (go.CompareTag("Enemy"))
        {
            coloredFirstPrefix =
                $"<color={Helpers.GetFactionColor(Faction.Enemy)}>{firstPrefix}</color>";
        }
        else if (go.CompareTag("Ally"))
        {
            coloredFirstPrefix =
                $"<color={Helpers.GetFactionColor(Faction.Ally)}>{firstPrefix}</color>";
        }
        else
        {
            coloredFirstPrefix = firstPrefix;
        }

        // Color target GameObject
        if (targetGo.CompareTag("Player"))
        {
            coloredSecondPrefix =
                $"<color={Helpers.GetFactionColor(Faction.Player)}>{secondPrefix}</color>";
        }
        else if (targetGo.CompareTag("Enemy"))
        {
            coloredSecondPrefix =
                $"<color={Helpers.GetFactionColor(Faction.Enemy)}>{secondPrefix}</color>";
        }
        else if (targetGo.CompareTag("Ally"))
        {
            coloredSecondPrefix =
                $"<color={Helpers.GetFactionColor(Faction.Ally)}>{secondPrefix}</color>";
        }
        else
        {
            coloredSecondPrefix = secondPrefix;
        }

        message = $"{FRAME_COUNT} {coloredFirstPrefix}->{coloredSecondPrefix} {message}";
        Debug.Log(message);
    }

    public static void Log(string message, GameObject go)
    {
        string coloredPrefix;
        string prefix = $"[{go.GetInstanceID()}]";

        if (go.CompareTag("Player"))
        {
            coloredPrefix = $"<color={Helpers.GetFactionColor(Faction.Player)}>{prefix}</color>";
        }
        else if (go.CompareTag("Enemy"))
        {
            coloredPrefix = $"<color={Helpers.GetFactionColor(Faction.Enemy)}>{prefix}</color>";
        }
        else if (go.CompareTag("Ally"))
        {
            coloredPrefix = $"<color={Helpers.GetFactionColor(Faction.Ally)}>{prefix}</color>";
        }
        else
        {
            coloredPrefix = prefix;
        }

        message = FRAME_COUNT + " " + coloredPrefix + " " + message;
        Debug.Log(message);
    }

    public static void Log(string message)
    {
        message = FRAME_COUNT + " " + message;

        Debug.Log(message);
    }

    public static void LogWarning(string message)
    {
        message = FRAME_COUNT + " " + message;

        Debug.LogWarning(message);
    }

    public static void LogError(string message)
    {
        message = FRAME_COUNT + " " + message;

        Debug.LogError(message);
    }
}
