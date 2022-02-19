using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public abstract class PlayerController : MonoBehaviour
{
    protected Controls controls;

    private void Awake()
    {
        controls = new Controls();
        transform.tag = "Player";
        DontDestroyOnLoad(this.gameObject);
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
