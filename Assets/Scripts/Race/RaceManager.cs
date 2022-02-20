using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    
    public static bool Started { get; private set; }
    public static string StartState { get; private set; } = "";

    public uint numLaps = 2; 
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

        cars.RemoveAll((a) => a.gameObject.activeSelf == false);
        Started = false;
        StartState = "3";
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
    }

    void Update()
    {
        cars.Sort((b,a) => a.Progress.CompareTo(b.Progress));
    }
}
