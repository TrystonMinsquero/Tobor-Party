using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuUI : MonoBehaviour
{
    public void PlayToborRacing()
    {
        SceneManager.LoadScene(Arcade.IsRunningInArcadeMode() ? "2P Lobby" : "Lobby");
    }
}
