using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{ 
    
    public Button activeButton;

    private void Start()
    {
        if(PlayerManager.instance != null)
            PlayerManager.SwitchAllActionMaps("UI");
        activeButton?.Select();
    }

    public void BackToLobby()
    {
        // implement later, i'm tired
        // if (PlayerManager.instance)
        // {
        //     foreach (Player player in PlayerManager.players)
        //     {
        //         if (player)
        //         {
        //             player.raceData.place = -1;
        //             player.raceData.finishTime = -1;
        //         }
        //     }
        // }
        
        if(PlayerManager.instance)
            PlayerManager.ClearAndDestroy();
        SceneManager.LoadScene("2P Lobby");
    }
}
