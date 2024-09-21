using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public KeyCode selectionKey = KeyCode.Mouse0;

    void Awake()
    {
        Instance = this;
    }

    public bool IsInputHeld()
    {
        return Input.GetKey(selectionKey);
    }
}
