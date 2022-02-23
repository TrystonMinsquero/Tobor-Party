using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public LeaderboardSlot[] slots = new LeaderboardSlot[4];
    public Button activeButton;

    private void Start()
    {
        foreach (var slot in slots)
        {
            slot.Hide();
        }

        if (PlayerManager.instance == null)
            return;
        
        PlayerManager.SwitchAllActionMaps("UI");
        Display();
    }

    public void Display()
    {
        activeButton?.Select();
        Populate();
        for (int i = 0; i < PlayerManager.playerCount; i++)
            slots[i].Show();
        GetComponent<Canvas>().enabled = true;
    }

    public void Populate()
    {

        List<Player> sortedPlayers = new List<Player>();
        
        foreach(Player player in PlayerManager.players)
            if(player)
                sortedPlayers.Add(player);
        
        sortedPlayers.Sort((player1, player2) => (int)((player1.raceData.finishTime - player2.raceData.finishTime) * 100));
        for(int i = 0; i < sortedPlayers.Count; i++)
            slots[i].Fill(sortedPlayers[i]);
            
    }
}


