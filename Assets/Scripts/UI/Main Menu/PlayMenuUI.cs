using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuUI : MonoBehaviour
{
    public void PlayToborRacing()
    {
        if (Environment.GetEnvironmentVariable("ARCADE_MODE") == null)
            SceneManager.LoadScene("Lobby");
        else
            SceneManager.LoadScene("2P Lobby");
    }
}
