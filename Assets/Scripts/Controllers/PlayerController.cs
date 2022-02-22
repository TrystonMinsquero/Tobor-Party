using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public abstract class PlayerController : MonoBehaviour
{
    public Controls controls { get; protected set; }

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

    public void SwitchActionMap(string actionMapName)
    {
        // Only enable correct action Map
        var playerInput = GetComponent<PlayerInput>();
        foreach(InputActionMap actionMap in playerInput.actions.actionMaps)
            actionMap?.Disable();
        if(playerInput.actions.FindActionMap(actionMapName) != null)
            playerInput.actions.FindActionMap(actionMapName).Enable();
        else
            Debug.LogWarning("Cannot find action map " + actionMapName);
    }
    
    
    public void OnPause(InputAction.CallbackContext ctx)
    {
        if(FindObjectOfType<PauseMenu>() == null)
            return;
        if (ctx.started)
        {
            if(!PauseMenu.IsPaused)
                PauseMenu.Pause();
            else
                PauseMenu.Resume();
        }
    }

    protected virtual void SubscribeToEvents() {}

    protected virtual void UnsubscribeToEvents() { }

    private void OnEnable()
    {
        controls.Enable();    
        SubscribeToEvents();;
    }

    private void OnDisable()
    {
        controls.Disable();
        UnsubscribeToEvents();
    }
}
