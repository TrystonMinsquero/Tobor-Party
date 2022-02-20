using UnityEngine.SceneManagement;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public JoinBox[] joinBoxes;

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
        SceneManager.LoadScene("RaceTrack");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
