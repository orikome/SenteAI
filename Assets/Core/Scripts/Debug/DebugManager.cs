using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;
    private static string FRAME_COUNT => $"[Frame {Time.frameCount}]";
    public bool enableLogging = true;

    // Color constants
    private static readonly string PLAYER_COLOR = "#800080"; // Purple
    private static readonly string ENEMY_COLOR = "#FFB3B3"; // Pastel Red
    private static readonly string ALLY_COLOR = "#B3FFB3"; // Pastel Green

    private void Awake()
    {
        Instance = this;
    }

    public void Log(string message, GameObject go, GameObject targetGo)
    {
        if (!enableLogging)
            return;

        string firstPrefix = $"[{go.GetInstanceID()}]";
        string secondPrefix = $"[{targetGo.GetInstanceID()}]";
        string coloredFirstPrefix,
            coloredSecondPrefix;

        // Color owner GameObject
        if (go.CompareTag("Player"))
        {
            coloredFirstPrefix = $"<color={PLAYER_COLOR}>{firstPrefix}</color>";
        }
        else if (go.CompareTag("Enemy"))
        {
            coloredFirstPrefix = $"<color={ENEMY_COLOR}>{firstPrefix}</color>";
        }
        else if (go.CompareTag("Ally"))
        {
            coloredFirstPrefix = $"<color={ALLY_COLOR}>{firstPrefix}</color>";
        }
        else
        {
            coloredFirstPrefix = firstPrefix;
        }

        // Color target GameObject
        if (targetGo.CompareTag("Player"))
        {
            coloredSecondPrefix = $"<color={PLAYER_COLOR}>{secondPrefix}</color>";
        }
        else if (targetGo.CompareTag("Enemy"))
        {
            coloredSecondPrefix = $"<color={ENEMY_COLOR}>{secondPrefix}</color>";
        }
        else if (targetGo.CompareTag("Ally"))
        {
            coloredSecondPrefix = $"<color={ALLY_COLOR}>{secondPrefix}</color>";
        }
        else
        {
            coloredSecondPrefix = secondPrefix;
        }

        message = $"{FRAME_COUNT} {coloredFirstPrefix}->{coloredSecondPrefix} {message}";
        Debug.Log(message);
    }

    public void Log(string message, GameObject go)
    {
        if (!enableLogging)
            return;

        string coloredPrefix;
        string prefix = $"[{go.GetInstanceID()}]";

        if (go.CompareTag("Player"))
        {
            coloredPrefix = $"<color={PLAYER_COLOR}>{prefix}</color>";
        }
        else if (go.CompareTag("Enemy"))
        {
            coloredPrefix = $"<color={ENEMY_COLOR}>{prefix}</color>";
        }
        else if (go.CompareTag("Ally"))
        {
            coloredPrefix = $"<color={ALLY_COLOR}>{prefix}</color>";
        }
        else
        {
            coloredPrefix = prefix;
        }

        message = FRAME_COUNT + " " + coloredPrefix + " " + message;
        Debug.Log(message);
    }

    public void Log(string message)
    {
        if (!enableLogging)
            return;

        message = FRAME_COUNT + " " + message;

        Debug.Log(message);
    }

    public void LogWarning(string message)
    {
        if (!enableLogging)
            return;

        message = FRAME_COUNT + " " + message;

        Debug.LogWarning(message);
    }

    public void LogError(string message)
    {
        if (!enableLogging)
            return;

        message = FRAME_COUNT + " " + message;

        Debug.LogError(message);
    }
}
