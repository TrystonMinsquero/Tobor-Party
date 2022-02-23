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


