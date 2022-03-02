using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public JoinBox[] joinBoxes;
    public Text actionText;

    public string _UIState = "Waiting for players to ready up...";
    public AudioSource countdownAudio;
    public AudioSource musicAudio;

    private float _initMusicVolume;
    private bool _countingDown;

    public void Start()
    {
        countdownAudio = GetComponent<AudioSource>();
        _initMusicVolume = musicAudio.volume;
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
        countdownAudio.Play();
        StartCoroutine(CountDown());
        StartCoroutine(FadeOutAudio(musicAudio, 3));
    }

    public void StopCountdown()
    {
        _countingDown = false;
        countdownAudio.Stop();
        musicAudio.volume = _initMusicVolume;
        StopAllCoroutines();
        _UIState = "Waiting for players to ready up...";
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float time)
    {
        if (audioSource)
        {
            float initVolume = audioSource.volume;
            while (audioSource.volume > .01f) {
                audioSource.volume -= initVolume * Time.deltaTime / time;
                yield return null;
            }
        }
        else
            Debug.LogWarning("Can't fade out null");
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
        if(PlayerManager.playerCount > 0)
            SceneManager.LoadScene("RaceTrack");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
