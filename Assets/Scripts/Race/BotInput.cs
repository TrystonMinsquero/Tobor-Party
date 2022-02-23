using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInput : MonoBehaviour
{
    private float checkpointPositionLerp = 0;

    void Start()
    {
        checkpointPositionLerp = Random.value;
    }

    public void UpdateInput(ref FrameInputs inputs, CheckpointUser cpUser)
    {
        var car = cpUser.car;
        var rb = car.rb;
        var checkpoint = cpUser.nextCheckpoint;
        var targetPos = Vector3.Lerp(checkpoint.leftGoal.position, checkpoint.rightGoal.position, checkpointPositionLerp);

        var targetDir = targetPos - rb.position;
        var rbDir = car.currentInputDirection;
        rbDir.y = 0;

        var angle = Vector3.SignedAngle(rbDir, targetDir, Vector3.up);

        inputs.gasInput = 1;
        inputs.steerInput = angle < 0 ? -1 : 1;
    }
}
