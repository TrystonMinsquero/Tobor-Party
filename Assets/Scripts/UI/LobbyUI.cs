using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public JoinBox[] joinBoxes;
    public Button startButton;

    public void Start()
    {
        startButton.Select();
    }

    public void Update()
    {
        UpdateJoinBoxes();
    }

    public void UpdateJoinBoxes()
    {
        for(int i = 0; i < joinBoxes.Length; i++)
        {
            if (PlayerManager.players[i] != null && !joinBoxes[i].hasPlayer)
                joinBoxes[i].AddPlayer(PlayerManager.players[i]);
            else if (PlayerManager.players[i] == null && joinBoxes[i].hasPlayer)
                joinBoxes[i].RemovePlayer(PlayerManager.players[i]);
        }
    }
    
    
    public void StartGame()
    {
        if(PlayerManager.playerCount > 0)
            SceneManager.LoadScene("RaceTrack");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
