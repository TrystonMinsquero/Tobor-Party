using System;
using UnityEngine.InputSystem;

public class LobbyController : PlayerController
{
    public event Action Ready = delegate {  };
    public event Action Leave = delegate {  };
    public event Action<float> ChangeSkin = delegate(float f) {  };
    public float Rotate { get; private set; }

    public void OnReady(InputAction.CallbackContext ctx) {if(ctx.started){Ready.Invoke();}}
    public void OnLeave(InputAction.CallbackContext ctx) {if(ctx.started){Leave.Invoke();}}
    public void OnChangeSkin(InputAction.CallbackContext ctx) { if(ctx.started) ChangeSkin.Invoke(ctx.ReadValue<float>()); }
    public void OnRotate(InputAction.CallbackContext ctx) { Rotate = ctx.ReadValue<float>(); }
}
