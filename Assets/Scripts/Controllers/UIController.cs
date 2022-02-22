using UnityEngine.InputSystem;

public class UIController : PlayerController
{
    public bool Ready { get; private set; }
    public bool Leave { get; private set; }
    public float ChangeSkin { private set; get; }
    public float Rotate { get; private set; }

    protected override void SubscribeToEvents()
    {
        controls.UI.Ready.started += OnReady;
        controls.UI.Leave.started += OnLeave;
    }
    protected override void UnsubscribeToEvents()
    {
        controls.UI.Ready.started -= OnReady;
        controls.UI.Leave.started -= OnLeave;
    }

    public void OnReady(InputAction.CallbackContext ctx) {Ready = ctx.started;}
    public void OnLeave(InputAction.CallbackContext ctx) { Leave = ctx.started; }
    public void OnChangeSkin(InputAction.CallbackContext ctx) { ChangeSkin = ctx.started ? ctx.ReadValue<float>() : 0; }
    public void OnRotate(InputAction.CallbackContext ctx) { Rotate = ctx.ReadValue<float>(); }
}
