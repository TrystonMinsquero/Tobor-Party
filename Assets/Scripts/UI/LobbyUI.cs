using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public JoinBox[] joinBoxes;
    public Text actionText;

    public string _UIState = "Waiting for players to ready up...";
    

    private bool _countingDown;
    
    
    public void Update()
    {
        bool hasPlayer = false;
        foreach(var joinBox in joinBoxes)
            if (joinBox.hasPlayer) hasPlayer = true;
        
        bool allReady = hasPlayer;
        
        foreach (var joinBox in joinBoxes)
            if (joinBox.hasPlayer && !joinBox.isReady)
                allReady = false;

        
        if(allReady && !_countingDown)
            StartCountdown();
        if(!allReady && _countingDown)
            StopCountdown();

        actionText.text = _UIState;
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

    public void StartCountdown()
    {
        _countingDown = true;
        StartCoroutine(CountDown());
    }

    public void StopCountdown()
    {
        _countingDown = false;
        StopAllCoroutines();
        _UIState = "Waiting for players to ready up...";
    }
    
    private IEnumerator CountDown()
    {
        for (int i = 3; i >= 1; i--)
        {
            _UIState = $"Game Starting in {i}";
            yield return new WaitForSeconds(1);
        }

        StartGame();
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
