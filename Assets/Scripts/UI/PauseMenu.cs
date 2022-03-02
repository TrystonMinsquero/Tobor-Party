using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public static bool IsPaused { get; private set; }

    public Button initialButton;

    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
        GetComponent<Canvas>().enabled = false;
        IsPaused = false;
    }

    public static void Pause()
    {
        if(IsPaused)
            return;
        IsPaused = true;
        Time.timeScale = 0;
        LevelManager.gameMusic?.Pause();
        RaceManager.instance?.countdownAudio?.Pause();
        AudioListener.pause = true;
        Instance.GetComponent<Canvas>().enabled = true;
        Instance.initialButton.Select();
        foreach (var player in PlayerManager.players)
        {
            player?.GetComponent<PlayerController>().SwitchActionMap("UI");
        }
    }
    
    public static void Resume()
    {
        if(!IsPaused)
            return;
        IsPaused = false;
        Time.timeScale = 1;
        LevelManager.gameMusic?.UnPause();
        RaceManager.instance?.countdownAudio?.UnPause();
        AudioListener.pause = false;
        if(Instance)
            Instance.GetComponent<Canvas>().enabled = false;
        foreach (var player in PlayerManager.players)
        {
            player?.GetComponent<PlayerController>().SwitchActionMap("Racing");
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public static void Leave()
    {
        IsPaused = false;
        AudioListener.pause = false;
        Time.timeScale = 1;
        if(Instance)
            Instance.GetComponent<Canvas>().enabled = false;
        //
        // foreach (var player in PlayerManager.players)
        // {
        //     player?.GetComponent<PlayerController>().SwitchActionMap("UI");
        // }
        //  
        
        PlayerManager.ClearAndDestroy();
        SceneManager.LoadScene("Lobby");
    }
    
    public static void Quit()
    {
        Application.Quit();
    }
}
