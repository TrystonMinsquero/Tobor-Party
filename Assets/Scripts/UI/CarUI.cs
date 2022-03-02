using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarUI : MonoBehaviour
{
    public Text startText;
    public Text placeText;
    public Text placeSuffixText;
    public TMP_Text lapText;
    public TMP_Text speedText;
    public TMP_Text checkpointTimeText;
    public TMP_Text lapTimerText;
    public Image[] itemBoxes;

    private ItemHolder holder;
    private CheckpointUser cpUser;
    private Rigidbody rb;
    private Checkpoint nextCheckPoint;

    public Transform speedometerNeedle;
    public Leaderboard leaderboard;

    public PositionNumberHandler positionNumberHandler;
    public Image ckptImage;

    private void Awake()
    {
        cpUser = GetComponentInParent<CheckpointUser>();
        rb = GetComponentInParent<Rigidbody>();
        holder = cpUser.GetComponent<ItemHolder>();
        
        leaderboard?.gameObject.SetActive(false);
        checkpointTimeText.text = "";
        nextCheckPoint = cpUser.nextCheckpoint;
    }

    private void OnEnable()
    {
        cpUser.ReachedCheckpoint += StartToShowCheckpointTime;
        cpUser.CompletedLap += StartToShowCheckpointTime;
    }
    
    private void OnDisable()
    {
        cpUser.ReachedCheckpoint -= StartToShowCheckpointTime;
        cpUser.CompletedLap -= StartToShowCheckpointTime;
    }

    private void Update()
    {
        // Cancel update if done
        if (cpUser.Laps >= RaceManager.instance.numLaps)
        {
            if (leaderboard && !leaderboard.gameObject.activeSelf)
                leaderboard?.gameObject.SetActive(true);
            leaderboard?.Display();
            return;
        }

        // Update Place text
        int place = RaceManager.instance.cars.IndexOf(cpUser) + 1;
        positionNumberHandler.UpdatePosition(place);
        //placeText.text = place.ToString();
        //placeSuffixText.text = GetPlaceSuffix(place);

        // Update start text, laptext, and speed text
        startText.text = RaceManager.StartState;
        lapText.text = (cpUser.Laps + 1) + "/" + RaceManager.instance.numLaps;
        speedText.text = $"{rb.velocity.magnitude:0}";

        // Update Wrong way text
        if (!cpUser.RightDirection)
            checkpointTimeText.text = "<color=red>Wrong Way!</color>";
        if (cpUser.RightDirection && checkpointTimeText.text == "<color=red>Wrong Way!</color>")
            checkpointTimeText.text = "";
        
        // Update Total Time
        TimeSpan time = TimeSpan.FromSeconds(Time.time - RaceManager.StartTime);
        if (!RaceManager.Started)
            lapTimerText.text = "00:00";
        else
            lapTimerText.text = time.TotalSeconds > 60 ? time.ToString("mm':'ss") : GetMillisecondTime(time);

        // Update Item box display
        for (int i = 0; i < itemBoxes.Length; i++)
        {
            if (i < holder.Items.Count)
            {
                itemBoxes[i].gameObject.SetActive(true);
                itemBoxes[i].sprite = holder.Items[i].itemImage;
            }
            else
            {
                itemBoxes[i].gameObject.SetActive(false);
                itemBoxes[i].sprite = null;
            }
        }
         
        // update speedometer
        RotateSpeedometer();
    }

    private void RotateSpeedometer()
    {
        float rot = 30; // starting rotation point is 30 degrees
        float rotMax = -180;

        float speedintervalsMultiplyer = rotMax / 20;
        rot += rb.velocity.magnitude * speedintervalsMultiplyer;
        
        Quaternion target = Quaternion.Euler(0, 0, rot);
        speedometerNeedle.localRotation = Quaternion.Slerp(speedometerNeedle.localRotation, target,  Time.deltaTime * 2);
    }

    public static string GetMillisecondTime(TimeSpan time)
    {
        string ms = time.Milliseconds.ToString();
        if (ms.Length >= 2)
            ms = ms.Substring(0,2);
        else if (ms.Length == 1)
            ms += "0";
        else if (ms.Length == 0)
            ms = "00";
        return time.ToString("ss'.'") + ms;
    }

    public static string GetPlaceSuffix(int place)
    {
        switch (place)
        {
            case 1: return "st";
            case 2: return "nd";
            case 3: return "rd";
            default: return "th";
        }
    }

    private void StartToShowCheckpointTime(float oldTime, float newTime)
    {
        StopCoroutine("ShowCheckpointTime");
        
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
        
        checkpointTimeText.text = ""; // reset text?
        ckptImage.enabled = true;
        ckptImage.transform.localScale = new Vector3(1, 0, 1);

        // play animation
        float lerpTime = 0, lerpDur = 0.15f;
        while (lerpTime < lerpDur)
        {
            float t = lerpTime / lerpDur;
            t = t * t * (3f - 2f * t);

            float y = Mathf.Lerp(0, 1, t);
            ckptImage.transform.localScale = new Vector3(1, y, 1);
            
            lerpTime += Time.deltaTime;
            yield return null;
        }
        ckptImage.transform.localScale = new Vector3(1, 1, 1);
            
        
        string rtColor = "";
        
        switch (prefix)
        {
            case '+': rtColor = "<color=\"red\">";
                break;
            case '-': rtColor = "<color=\"green\">";
                break;
            default: rtColor = "<color=#939397>"; 
                break;
        }
        
        //checkpointTimeText.text = "CHECKPOINT  " + (prefix == '-' ? $"{time:0.00}s" : "" + prefix + $"{time:0.00}s");
        checkpointTimeText.text = "CHECKPOINT  " + rtColor + (prefix == '-' ? $"{time:0.00}s" : "" + prefix + $"{time:0.00}s");
        
        yield return new WaitForSeconds(1.5f);
        
        // play animation
        lerpTime = 0;
        lerpDur = 0.1f;
        while (lerpTime < lerpDur)
        {
            float t = lerpTime / lerpDur;
            t = t * t * (3f - 2f * t);

            float y = Mathf.Lerp(1, 0, t);
            ckptImage.transform.localScale = new Vector3(1, y, 1);
            
            lerpTime += Time.deltaTime;
            yield return null;
        }
        ckptImage.transform.localScale = new Vector3(1, 0, 1);
        
        checkpointTimeText.text = "";
        ckptImage.enabled = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
