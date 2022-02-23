using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public LeaderboardSlot[] slots = new LeaderboardSlot[8];
    public Button doneButton;

    public void Display()
    {
        doneButton.Select();
        Populate();
        for (int i = 0; i < PlayerManager.playerCount; i++)
            slots[i].Show();
        GetComponent<Canvas>().enabled = true;
    }

    public void Populate()
    {
        int slotCount = 0;
        foreach(Player player in PlayerManager.players)
        {
            if (player != null && PlayerManager.Contains(player))
            {
                slots[slotCount].Fill(player);
                slotCount++;
            }
        }
    }
}


