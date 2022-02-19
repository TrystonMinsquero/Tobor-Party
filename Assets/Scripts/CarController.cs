using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CarController : PlayerController
{
    public Vector2 MoveInput { get; private set; }

    public void OnMove(InputAction.CallbackContext ctx) { MoveInput = ctx.ReadValue<Vector2>(); }

}
