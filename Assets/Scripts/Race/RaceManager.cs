using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    
    public static bool Started { get; private set; }
    public static bool Finished { get; private set; }
    public static float StartTime { get; private set; }
    public static string StartState { get; private set; } = "";

    public uint numLaps = 1; 
    public List<CheckpointUser> cars = new List<CheckpointUser>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (cars == null || cars.Count == 0)
        {
            Started = true;
            StartState = "";
            return;
        }

        // Removes cars that are disable
        cars.RemoveAll((a) => a.gameObject.activeSelf == false);
        
        // Link Checkpoint User to Player
        foreach(Player player in PlayerManager.players)
            if (player && player.playerObject.TryGetComponent<CheckpointUser>(out var cpUser))
                cpUser.FinishedRace += data => player.AddRaceData(data); 
                

        Started = false;
        Finished = false;
        StartState = "3";
        StartTime = Time.time;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        for (int i = 3; i >= 1; i--)
        {
            StartState = $"{i}";
            yield return new WaitForSeconds(1);
        }

        StartState = "GO!";
        Started = true;
        yield return new WaitForSeconds(1);

        StartState = "";
        StartTime = Time.time;
    }

    IEnumerator EndGame()
    {
        foreach (var player in PlayerManager.players)
            if (player != null && player.playerObject != null)
            {
                var cpUser = player.playerObject.GetComponent<CheckpointUser>();
                cpUser.FinishedRace -= data => player.AddRaceData(data);
                player.AddRaceData(cpUser.GetRaceData());
            }
                
        
        yield return new WaitForSeconds(2);
        
        SceneManager.LoadScene("Winning Scene");
    }

    void Update()
    {
        cars.Sort((b,a) => a.Progress.CompareTo(b.Progress));

        bool allFinished = true;
        foreach (CheckpointUser cpUser in cars)
            if (cpUser.Laps < numLaps)
                allFinished = false;

        if (allFinished && !Finished)
        {
            Finished = true;
            StartCoroutine(EndGame());
        }

    }
}
