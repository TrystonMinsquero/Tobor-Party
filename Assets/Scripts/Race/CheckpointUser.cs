using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointUser : MonoBehaviour
{
    public bool RightDirection { get; private set; } = true;
    public Checkpoint currentCheckpoint;
    public Checkpoint nextCheckpoint;
    public Car car;

    void Start()
    {
        currentCheckpoint = Checkpoints.Instance.checkpoints[0];
        nextCheckpoint = Checkpoints.Instance.checkpoints[1];

        car = GetComponent<Car>();
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

            }
        }
    }

    void Update()
    {
        var vel = car.rb.velocity;
        
        if (vel.sqrMagnitude > 0.4f)
        {
            var dir = nextCheckpoint.transform.position - currentCheckpoint.transform.position;
            RightDirection = Vector3.Dot(vel, dir) > 0;
        }
    }
}
