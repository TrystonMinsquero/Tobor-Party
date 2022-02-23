using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardSlot : MonoBehaviour
{
    public TMP_Text playerNum;
    public TMP_Text playerPlace;
    public TMP_Text playerTime;



    public void Fill(Player player)
    {
        if (PlayerManager.GetIndex(player) < 0)
            return;
        playerNum.text = "P" + (PlayerManager.GetIndex(player) + 1);
        playerPlace.text = "" + player.raceData.place + CarUI.GetPlaceSuffix(player.raceData.place);
        var timeSpan = TimeSpan.FromSeconds(player.raceData.finishTime);
        playerTime.text = timeSpan.TotalSeconds >= 60
            ? timeSpan.ToString("mm':'ss'.'FF")
            : CarUI.GetMillisecondTime(timeSpan);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void LeaveToLobby()
    {
        foreach (var player in PlayerManager.players)
        {
            Destroy(player?.gameObject);
        }
        Destroy(PlayerManager.instance.gameObject);
        
        SceneManager.LoadScene("Lobby");
    }

}


