using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 15, -8);

    [SerializeField]
    private float smoothSpeed = 10f;

    [Header("View Settings")]
    [SerializeField]
    private float rotationX = 45f;

    [SerializeField]
    private float minZoom = 8f;

    [SerializeField]
    private float maxZoom = 20f;

    [SerializeField]
    private float zoomSpeed = 4f;

    [SerializeField]
    private float currentZoom = 15f;

    private void Start()
    {
        if (target == null)
            AgentLogger.LogWarning("No target assigned to CameraManager!");

        // Set initial position and rotation
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Handle zoom
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom = Mathf.Clamp(currentZoom - scrollInput * zoomSpeed, minZoom, maxZoom);

        // Calculate position with zoom
        Vector3 desiredPosition =
            target.position + Quaternion.Euler(0, 0, 0) * new Vector3(0, offset.y, -currentZoom);

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
        transform.position = smoothedPosition;

        // Maintain fixed rotation
        transform.rotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
