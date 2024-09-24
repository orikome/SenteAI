using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerMesh;
    public Camera mainCamera;
    private Vector3 currentLookDirection;
    public static PlayerLook Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (playerMesh == null)
        {
            Debug.LogError("Player mesh is not assigned!");
            return;
        }

        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float rayLength))
        {
            Vector3 pointToLook = ray.GetPoint(rayLength);

            Vector3 direction = pointToLook - playerMesh.position;
            direction.y = 0; // XZ plane

            playerMesh.rotation = Quaternion.LookRotation(direction);

            currentLookDirection = direction.normalized;
        }
    }

    public Vector3 GetLookDirection()
    {
        return currentLookDirection;
    }
}
