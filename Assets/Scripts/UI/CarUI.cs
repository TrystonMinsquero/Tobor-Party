using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUI : MonoBehaviour
{
    public Text startText;
    public Text placeText;
    public Text placeSuffixText;
    public Text lapText;
    public Text speedText;
    public Text checkpointTimeText;
    public Gradient checkPointGradient;

    private CheckpointUser cpUser;
    private Rigidbody rb;
    private Checkpoint nextCheckPoint;

    private void Awake()
    {
        cpUser = GetComponent<CheckpointUser>();
        rb = GetComponent<Rigidbody>();
        checkpointTimeText.text = "";
        nextCheckPoint = cpUser.nextCheckpoint;
    }

    private void OnEnable()
    {
        cpUser.ReachedCheckpoint += StartToShowCheckpointTime;
    }

    private void Update()
    {
        int place = RaceManager.instance.cars.IndexOf(cpUser) + 1;
        placeText.text = place.ToString();
        placeSuffixText.text = GetPlaceSuffix(place);

        startText.text = RaceManager.StartState;
        lapText.text = (cpUser.Laps + 1) + "/" + RaceManager.instance.numLaps;
        speedText.text = $"{rb.velocity.magnitude:0.0}";

        if (cpUser.currentCheckpoint == nextCheckPoint)
        {
            
        }
        
    }

    private string GetPlaceSuffix(int place)
    {
        switch (place)
        {
            case 1: return "st";
                break;
            case 2: return "nd";
                break;
            case 3: return "rd";
                break;
            default: return "th";
        }
    }

    private void StartToShowCheckpointTime(float oldTime, float newTime)
    {
        float time = newTime - oldTime;
        char prefix = ' ';
        if (oldTime < 0)
            time = newTime;
        else if (time > 0)
            prefix = '+';
        else if (time < 0)
            prefix = '-';


        StartCoroutine(ShowCheckpointTime(time, prefix));
    }

    private IEnumerator ShowCheckpointTime(float time, char prefix)
    {
        switch (prefix)
        {
            case '+': checkpointTimeText.color = Color.red;
                break;
            case '-': checkpointTimeText.color = Color.green;
                break;
            default: checkpointTimeText.color = Color.gray;
                break;
        }
        
        checkpointTimeText.text = prefix == '-' ? $"{time:0.00}s" : "" + prefix + $"{time:0.00}s";
        yield return new WaitForSeconds(1.5f);
        checkpointTimeText.text = "";
    }
}
