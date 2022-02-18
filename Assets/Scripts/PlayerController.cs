using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Vector2 movementInput;
    private Controls controls;

    private PlayerController _playerController;

    private void Awake()
    {
        controls = new Controls();

        _playerController = GetComponent<PlayerController>();
        
    }

    private void FixedUpdate()
    {
        transform.Translate(movementInput);
    }

    public void OnMove(InputAction.CallbackContext ctx) { movementInput = ctx.ReadValue<Vector2>(); }

    private void OnEnable()
    {
        controls.Enable();    
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
