using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointUser : MonoBehaviour
{
    public bool RightDirection { get; private set; } = true;
    public float Progress { get; private set; } = 0;
    public int Laps { get; private set; } = 0;
    public float StartTime { get; private set; } = 0;

    public Checkpoint currentCheckpoint;
    public Checkpoint nextCheckpoint;
    public Car car;

    void Start()
    {
        currentCheckpoint = Checkpoints.Instance.checkpoints[0];
        nextCheckpoint = Checkpoints.Instance.checkpoints[1];

        car = GetComponent<Car>();

        // Todo: integrate this
        ResetTimer();
    }

    public void ResetTimer()
    {
        StartTime = Time.time;
    }

    public void CheckpointReached(Checkpoint checkpoint)
    {
        if (checkpoint.index == (currentCheckpoint.index + 1) % Checkpoints.Instance.checkpoints.Count)
        {
            currentCheckpoint = checkpoint;
            nextCheckpoint =
                Checkpoints.Instance.checkpoints[
                    (currentCheckpoint.index + 1) % Checkpoints.Instance.checkpoints.Count];

            Debug.Log($"Checkpoint reached: {checkpoint.index}");

            if (checkpoint.index == 0)
            {
                Laps++;
                Debug.Log($"Finished lap in {Time.time - StartTime}");
                ResetTimer();
            }
        }
    }

    void Update()
    {
        var vel = car.rb.velocity;
        var dir = nextCheckpoint.transform.position - currentCheckpoint.transform.position;

        if (vel.sqrMagnitude > 0.4f)
        {
            RightDirection = Vector3.Dot(vel, dir) > 0;
        }

        float progress = Laps;
        float factor = 1f / Checkpoints.Instance.checkpoints.Count;
        float percent = Vector3.Dot(transform.position - currentCheckpoint.transform.position, dir) / dir.sqrMagnitude;
        percent = Mathf.Clamp01(percent);
        progress += currentCheckpoint.index * factor;
        progress += factor * percent;
        progress += factor * percent;
        Progress = progress;
    }
}
