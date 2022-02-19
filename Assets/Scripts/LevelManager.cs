using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class LevelManager : MonoBehaviour
{
    public List<PlayerSpawner> playerSpawners;

    private void Awake()
    {
        Stack<PlayerSpawner> spawnerStack = new Stack<PlayerSpawner>();
        foreach(PlayerSpawner playerSpawner in playerSpawners) {spawnerStack.Push(playerSpawner);}
        
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
        foreach (var playerController in playerControllers)
        {
            bool destroyed = spawnerStack.Peek().TryToSpawnWith(playerController);
            // Remove from list if it destroyed itself (successfully spawned)
            if (destroyed)
                spawnerStack.Pop();
        }
        
        
        // disable objects that weren't assigned
        foreach (PlayerSpawner playerSpawner in spawnerStack)
        {
            Debug.Log(playerSpawner);
            if(playerSpawner)
                playerSpawner.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        
    }
}
