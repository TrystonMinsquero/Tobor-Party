using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public LeaderboardSlot[] slots = new LeaderboardSlot[4];

    private void Start()
    {
        foreach (var slot in slots)
        {
            slot.Hide();
        }

        if (PlayerManager.instance == null)
            return;
        
        Display();
    }

    public void Display()
    {
        Populate();
        int i = 0;
        foreach (var player in PlayerManager.players)
        {
            if (player && player.raceData != null && player.raceData.place > 0 && player.raceData.finishTime >= 0)
            {
                slots[i].Show();
                i++;
            }
        }
        GetComponent<Canvas>().enabled = true;
    }

    private void Populate()
    {
        List<Player> sortedPlayers = new List<Player>();
        
        foreach(Player player in PlayerManager.players)
            if(player && player.raceData != null && player.raceData.place > 0 && player.raceData.finishTime >= 0)
                sortedPlayers.Add(player);
        
        sortedPlayers.Sort((player1, player2) => player1.raceData.place - player2.raceData.place);
        for(int i = 0; i < sortedPlayers.Count; i++)
            slots[i].Fill(sortedPlayers[i]);
            
    }
}


