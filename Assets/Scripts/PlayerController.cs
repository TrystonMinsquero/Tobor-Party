using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : Player
{
    private Vector2 movementInput;
    private Controls controls;

    private PlayerInput _playerInput;

    private void Awake()
    {
        controls = new Controls();
        transform.tag = "Player";
        DontDestroyOnLoad(this.gameObject);

        _playerInput = GetComponent<PlayerInput>();
        
    }

    private void OnEnable()
    {
        controls.Enable();    
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
