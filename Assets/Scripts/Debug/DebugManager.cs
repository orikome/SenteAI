using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;
    public GameObject debugTextPrefab;
    public Transform cameraTransform;

    private void Awake()
    {
        Instance = this;
    }

    public void Log(Transform agentTransform, string message, Color color)
    {
        GameObject debugTextObj = Instantiate(
            debugTextPrefab,
            agentTransform.position + Vector3.up * 2,
            Quaternion.identity,
            agentTransform
        );
        DebugText debugText = debugTextObj.GetComponent<DebugText>();
        debugText.SetText(message, color);
    }
}
