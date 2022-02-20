using UnityEngine.InputSystem;

public class UIController : PlayerController
{
    public float Rotate { get; private set; }

    public void OnRotate(InputAction.CallbackContext ctx) { Rotate = ctx.ReadValue<float>(); }
}
