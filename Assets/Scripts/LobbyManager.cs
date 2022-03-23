using System;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    //static variables
    public static LobbyManager instance;

    //true if in lobby menu (not controls, etc.)
    public static bool CanJoinLeave
    {
        get { return instance; }
    }

    public static bool playerHasJoined = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        //MusicManager.StartMusic();
    }

    private void Update()
    {
        if (PlayerManager.playerCount > 0)
            playerHasJoined = true;
    }

    private void OnDestroy()
    {
        playerHasJoined = false;
    }
}
