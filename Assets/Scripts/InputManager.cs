using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    private static InputAction mousePositionAction;
    private static InputAction mouseAction;

    public static Vector2 MousePosition => mousePositionAction != null ? mousePositionAction.ReadValue<Vector2>() : Vector2.zero;
    public static bool WasLeftMouseButtonPressed => mouseAction != null && mouseAction.WasPressedThisFrame();
    public static bool WasLeftMouseButtonReleased => mouseAction != null && mouseAction.WasReleasedThisFrame();
    public static bool IsLeftMousePressed => mouseAction != null && mouseAction.IsPressed();

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        Debug.Log($"[InputManager] Awake called. PlayerInput = {PlayerInput != null}");
        if (PlayerInput != null)
        {
            mousePositionAction = PlayerInput.actions["MousePosition"];
            mouseAction = PlayerInput.actions["Mouse"];
            Debug.Log($"[InputManager] mouseAction: {mouseAction != null}");
        }
    }
}
