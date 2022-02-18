using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CarController : Player
{
    public Vector2 MoveInput { get; private set; }
    private Controls _controls;

    private PlayerInput _playerInput;

    void Awake()
    {
        _controls = new Controls();
        _playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }

    public void OnMove(InputAction.CallbackContext ctx) { MoveInput = ctx.ReadValue<Vector2>(); }

}
