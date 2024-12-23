using UnityEngine;

public class Player : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerMesh;
    public Camera mainCamera;
    private Vector3 currentLookDirection;
    public static Player Instance { get; private set; }
    private Agent _playerAgent;
    public PlayerWeaponRecoil PlayerWeaponRecoil { get; private set; }
    public KeyCode selectionKey = KeyCode.Mouse0;

    void Awake()
    {
        Instance = this;
        _playerAgent = GetComponent<Agent>();
        PlayerWeaponRecoil = GetComponentInChildren<PlayerWeaponRecoil>();
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

    public bool IsInputHeld()
    {
        return Input.GetKey(selectionKey);
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

    public Vector3 GetClosestEnemyDirection()
    {
        PlayerMetrics playerMetrics = (PlayerMetrics)_playerAgent.Metrics;
        var nearestEnemy = playerMetrics.FindClosestEnemyToPlayer();
        if (nearestEnemy != null)
        {
            return (nearestEnemy.position - _playerAgent.firePoint.position).normalized;
        }

        return _playerAgent.firePoint.forward;
    }

    public Vector3 GetMouseLookDirection()
    {
        return currentLookDirection;
    }
}
