using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSlot : MonoBehaviour
{
    public Text playerNum;
    public Text playerPlace;
    public Text playerTime;



    public void Fill(Player player)
    {
        if (player == null)
            return;
        playerNum.text = "" + (PlayerManager.GetIndex(player) + 1);
        playerPlace.text = "" + player.raceData.place;
        playerTime.text = "" + player.raceData.lapTimes.Sum();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
