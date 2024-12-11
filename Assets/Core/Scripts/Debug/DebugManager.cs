using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;
    public GameObject debugTextPrefab;
    private static string FRAME_COUNT => $"[Frame {Time.frameCount}]";
    public bool enableLogging = true;

    private void Awake()
    {
        Instance = this;
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

    public void SpawnTextLog(Transform agentTransform, string message, Color color)
    {
        Vector3 position = OrikomeUtils.GeneralUtils.GetPositionWithOffset(
            agentTransform,
            Random.Range(-4.0f, 4.0f),
            Random.Range(4.0f, 4.0f),
            Random.Range(-4.0f, 4.0f)
        );

        GameObject debugTextObj = Instantiate(
            debugTextPrefab,
            position,
            Quaternion.identity,
            agentTransform
        );

        DebugText debugText = debugTextObj.GetComponent<DebugText>();
        debugText.SetText(message, color);
    }
}
