using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;
    public GameObject debugTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Log(Transform agentTransform, string message, Color color)
    {
        Vector3 position = OrikomeUtils.GeneralUtils.GetPositionWithOffset(
            agentTransform,
            Random.Range(-1.0f, 1.0f),
            Random.Range(3.5f, 3.5f),
            Random.Range(-2.0f, 2.0f)
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
