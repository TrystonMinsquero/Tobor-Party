using System;
using UnityEngine.InputSystem;

public class UIController : PlayerController
{
    public event Action Ready = delegate {  };
    public event Action Leave = delegate {  };
    public float ChangeSkin { get; private set; }
    public float Rotate { get; private set; }

    public void OnReady(InputAction.CallbackContext ctx) {if(ctx.started){Ready.Invoke();}}
    public void OnLeave(InputAction.CallbackContext ctx) {if(ctx.started){Leave.Invoke();}}
    public void OnChangeSkin(InputAction.CallbackContext ctx) { ChangeSkin = ctx.started ? ctx.ReadValue<float>() : 0; }
    public void OnRotate(InputAction.CallbackContext ctx) { Rotate = ctx.ReadValue<float>(); }
}
