using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerObject))]
public class PlayerSpawner : MonoBehaviour
{
    public bool TryToSpawnWith(PlayerController player)
    {
        PlayerObject playerObject = GetComponent<PlayerObject>();
        
        if (!playerObject.AssignController(player))
            return false;
        
        Destroy(this);
        return true;
    }
}
