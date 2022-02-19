using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class LevelManager : MonoBehaviour
{
    // only necessary for order of players
    public PlayerSpawner[] playerSpawners;

    private void Awake()
    {
        // if someone forgets to start spawners on
        if (playerSpawners == null)
        {
            Debug.LogWarning("You should assign playerSpawners");
            playerSpawners = FindObjectsOfType<PlayerSpawner>();
        }
        
        // convert to stack
        Stack<PlayerSpawner> spawnerStack = new Stack<PlayerSpawner>();
        foreach(PlayerSpawner playerSpawner in playerSpawners.Reverse()) {spawnerStack.Push(playerSpawner);}

        // if came from the lobby
        if (PlayerManager.instance)
        {
            // give all players a player object
            foreach (var playerController in PlayerManager.players)
            {
                bool destroyed = spawnerStack.Peek().TryToSpawnWith(playerController?.GetComponent<PlayerController>());
                // Remove from list if it destroyed itself (successfully spawned)
                if (destroyed)
                    spawnerStack.Pop();
            }
        }

        // disable objects that weren't assigned
        foreach (PlayerObject playerObject in FindObjectsOfType<PlayerObject>())
        {
            if(!playerObject.HasController())
                playerObject.gameObject.SetActive(false);
        }

        PlayerInputManager.instance.splitScreen = true;
    }

}
