using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CarController : PlayerController
{
    public float SteerInput { get; private set; }
    public float GasBreakInput { get; private set; }
    
    public bool DriftInput { get; private set; }
    public bool UseItemInput { get; private set; }
    
    public Vector2 LookInput { get; private set; }


    public void OnSteer(InputAction.CallbackContext ctx) { SteerInput = ctx.ReadValue<float>(); }
    public void OnGasBreak(InputAction.CallbackContext ctx) { GasBreakInput = ctx.ReadValue<float>(); }
    public void OnDrift(InputAction.CallbackContext ctx) { DriftInput = ctx.ReadValue<float>() > 0.1f; }
    public void OnUseItem(InputAction.CallbackContext ctx) { UseItemInput = ctx.ReadValue<float>() > 0.1f;  }
    public void OnLook(InputAction.CallbackContext ctx) { LookInput = ctx.ReadValue<Vector2>(); }


}
