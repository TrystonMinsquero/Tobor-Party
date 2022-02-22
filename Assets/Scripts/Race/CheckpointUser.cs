using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointUser : MonoBehaviour
{
    public bool RightDirection { get; private set; } = true;
    public float Progress { get; private set; } = 0;
    public int Laps { get; private set; } = 0;
    public float LastLapStartTime { get; private set; } = 0;
    public float LastCheckpointStartTime { get; private set; } = 0;

    private Dictionary<Checkpoint, float> checkPointTimes;
    private List<float> lapTimes;

    // <old time, new time>
    public event Action<float, float> ReachedCheckpoint = delegate(float f, float f1) {  };
    // <old time, new time>
    public event Action<float, float> CompletedLap = delegate(float f, float f1) {  };

    public Checkpoint currentCheckpoint;
    public Checkpoint nextCheckpoint;
    public Car car;

    void Start()
    {
        currentCheckpoint = Checkpoints.Instance.checkpoints[0];
        nextCheckpoint = Checkpoints.Instance.checkpoints[1];
        lapTimes = new List<float>();

        checkPointTimes = new Dictionary<Checkpoint, float>();
        foreach (Checkpoint cp in Checkpoints.Instance.checkpoints)
            checkPointTimes[cp] = -1;

        car = GetComponent<Car>();

    }


    public void CheckpointReached(Checkpoint checkpoint)
    {
        if (checkpoint.index == (currentCheckpoint.index + 1) % Checkpoints.Instance.checkpoints.Count)
        {
            UpdateCheckpoint(currentCheckpoint, Time.time - LastCheckpointStartTime);
            currentCheckpoint = checkpoint;
            nextCheckpoint =
                Checkpoints.Instance.checkpoints[
                    (currentCheckpoint.index + 1) % Checkpoints.Instance.checkpoints.Count];

            LastCheckpointStartTime = Time.time;
            Debug.Log($"Checkpoint reached: {checkpoint.index}");

            if (checkpoint.index == 0)
            {
                float newTime = Time.time - LastLapStartTime;

                if(lapTimes.Count == 0)
                    CompletedLap.Invoke(-1, newTime);
                else
                {
                    float bestTime = lapTimes.Min((f => f));
                    CompletedLap.Invoke(bestTime, newTime);
                }
                
                lapTimes.Add(newTime);
                Laps++;
                Debug.Log($"Finished lap in {Time.time - LastLapStartTime}");
            }
        }
    }

    private float wrongDirectionTime = 0;
    void Update()
    {
        var vel = car.rb.velocity;
        var dir = nextCheckpoint.transform.position - currentCheckpoint.transform.position;

        bool rightDir = vel.sqrMagnitude > 0.4f && Vector3.Dot(vel, dir) < 0;
        if (rightDir)
        {
            wrongDirectionTime += Time.deltaTime;
            if (wrongDirectionTime > 2f)
                RightDirection = false;
        }
        else
        {
            wrongDirectionTime = 0;
            RightDirection = true;
        }

        float progress = Laps;
        float factor = 1f / Checkpoints.Instance.checkpoints.Count;
        float percent = Vector3.Dot(transform.position - currentCheckpoint.transform.position, dir) / dir.sqrMagnitude;
        percent = Mathf.Clamp01(percent);
        progress += currentCheckpoint.index * factor;
        progress += factor * percent;
        
        Progress = progress;
    }

    public void UpdateCheckpoint(Checkpoint cp, float newTime)
    {
        ReachedCheckpoint?.Invoke(checkPointTimes[cp], newTime);
        checkPointTimes[cp] = checkPointTimes[cp] < 0 ? newTime : Mathf.Max(newTime, checkPointTimes[cp]);
        LastCheckpointStartTime = Time.time;
    }


}
