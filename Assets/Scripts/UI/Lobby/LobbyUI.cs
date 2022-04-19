using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public JoinBox[] joinBoxes;
    public TMP_Text actionText;

    public string _UIState = "Waiting for players to ready up...";
    
    private AudioSource _countdownAudio;
    private AudioSource _musicAudio;
    private float _initMusicVolume;
    private bool _countingDown;

    public void Start()
    {
        _countdownAudio = GetComponent<AudioSource>();
        _musicAudio = MusicManager.currentSong.source;
        _initMusicVolume = MusicManager.currentSong.volume;
        foreach (var box in joinBoxes)
            box.EmptySlot();
    }

    public void Update()
    {
        bool hasPlayer = false;
        foreach(var joinBox in joinBoxes)
            if (joinBox.hasPlayer) hasPlayer = true;
        
        bool allReady = hasPlayer;
        
        foreach (var joinBox in joinBoxes)
            if (joinBox.hasPlayer && !joinBox.isReady)
                allReady = false;

        
        if(allReady && !_countingDown)
            StartCountdown();
        if(!allReady && _countingDown)
            StopCountdown();

        actionText.text = _UIState;
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

    public void StartCountdown()
    {
        _countingDown = true;
        _countdownAudio.Play();
        StartCoroutine(CountDown());
        MusicManager.FadeSongTo(MusicManager.currentSong.name, 3.5f, .03f, .8f);
    }

    public void StopCountdown()
    {
        _countingDown = false;
        _countdownAudio.Stop();
        _musicAudio.volume = _initMusicVolume;
        StopAllCoroutines();
        MusicManager.instance.StopAllCoroutines();
        _UIState = "Waiting for players to ready up...";
    }

    private IEnumerator CountDown()
    {
        for (int i = 3; i >= 0; i--)
        {
            _UIState = $"Game Starting in {i}";
            yield return new WaitForSeconds(1);
        }

        StartGame();
    }
    
    
    public void StartGame()
    {
        if (PlayerManager.playerCount > 0)
        {
            SceneManager.LoadScene("RaceTrack");
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
