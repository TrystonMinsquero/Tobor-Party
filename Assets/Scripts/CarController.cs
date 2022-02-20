using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CarController : PlayerController
{
    public float SteerInput { get; private set; }
    public float GasBreakInput { get; private set; }

    public void OnSteer(InputAction.CallbackContext ctx) { SteerInput = ctx.ReadValue<float>(); }
    public void OnGasBreak(InputAction.CallbackContext ctx) { GasBreakInput = ctx.ReadValue<float>(); }

}
