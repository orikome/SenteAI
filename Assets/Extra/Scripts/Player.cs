using TMPro;
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
    public Transform actionBar;
    public GameObject actionSlotPrefab;

    void Awake()
    {
        Instance = this;
        _playerAgent = GetComponent<Agent>();
        PlayerWeaponRecoil = GetComponentInChildren<PlayerWeaponRecoil>();
    }

    private void Start()
    {
        CreateActionSlots();
    }

    private void Update()
    {
        if (playerMesh == null)
        {
            AgentLogger.LogError("Player mesh is not assigned!");
            return;
        }

        HandleMouseLook();
    }

    public bool IsInputHeld()
    {
        return Input.GetKey(selectionKey);
    }

    private void CreateActionSlots()
    {
        foreach (var action in _playerAgent.Actions)
        {
            AgentLogger.LogWarning(action.name);
            var actionSlot = Instantiate(actionSlotPrefab, actionBar);
            actionSlot.transform.SetParent(actionBar);
            actionSlot.GetComponentInChildren<TextMeshProUGUI>().text = Helpers.CleanName(
                action.name
            );
        }
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

    public Vector3 GetMouseLookDirection()
    {
        return currentLookDirection;
    }
}
