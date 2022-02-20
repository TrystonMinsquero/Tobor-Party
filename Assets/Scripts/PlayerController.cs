using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public abstract class PlayerController : MonoBehaviour
{
    protected Controls controls;

    private void Awake()
    {
        // Check for other components to see if they have already instantiated
        foreach (PlayerController pc in GetComponents<PlayerController>())
        {
            if (pc.controls != null)
                controls = pc.controls;
        }
        
        if(controls == null)
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
