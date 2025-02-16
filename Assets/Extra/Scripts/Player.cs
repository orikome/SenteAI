using System.Collections.Generic;
using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
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
        private Dictionary<KeyCode, AgentAction> _actionSlotBindings =
            new Dictionary<KeyCode, AgentAction>();
        private List<ActionSlot> _actionSlots = new List<ActionSlot>();

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
            HandleActionSlotInputs();
        }

        public bool IsInputHeld()
        {
            return Input.GetKey(selectionKey);
        }

        private void CreateActionSlots()
        {
            int slotNumber = 1;
            foreach (var action in _playerAgent.Actions)
            {
                var actionSlotObj = Instantiate(actionSlotPrefab, actionBar);
                var actionSlot = actionSlotObj.GetComponent<ActionSlot>();
                actionSlot.Initialize(action, $"{slotNumber}. {Helpers.CleanName(action.name)}");
                _actionSlots.Add(actionSlot);

                // Bind number key to action
                if (slotNumber <= 9)
                {
                    KeyCode keyCode = (KeyCode)((int)KeyCode.Alpha1 + (slotNumber - 1));
                    _actionSlotBindings.Add(keyCode, action);
                }

                slotNumber++;
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

        private void HandleActionSlotInputs()
        {
            foreach (var binding in _actionSlotBindings)
            {
                if (Input.GetKeyDown(binding.Key))
                {
                    var brain = _playerAgent.GetModule<Brain>();
                    brain.SetAction(binding.Value);
                }
            }
        }

        public Vector3 GetMouseLookDirection()
        {
            return currentLookDirection;
        }
    }
}
