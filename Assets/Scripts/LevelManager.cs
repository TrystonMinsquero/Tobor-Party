using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    // only necessary for order of players
    public PlayerSpawner[] playerSpawners;

    public static AudioSource gameMusic;

    private void Awake()
    {
        Instance = this;
        
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
            foreach (var player in PlayerManager.players)
            {
                player?.SetUp(spawnerStack.Peek().GetComponent<PlayerObject>());
                bool destroyed = spawnerStack.Peek().TryToSpawnWith(player?.GetComponent<PlayerController>());
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

        gameMusic = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        gameMusic = null;
    }
}
