using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInput : MonoBehaviour
{
    public bool attackPlayer = false;

    private float checkpointPositionLerp = 0;
    private Car target;

    void Start()
    {
        checkpointPositionLerp = Random.value;

        if (PlayerManager.playerCount > 0)
        {
            var plr = PlayerManager.players[Random.Range(0, PlayerManager.playerCount)];
            target = (Car) plr.playerObject;
        }
    }

    public void UpdateInput(ref FrameInputs inputs, CheckpointUser cpUser)
    {
        var car = cpUser.car;
        var rb = car.rb;
        var checkpoint = cpUser.nextCheckpoint;
        var targetPos = Vector3.Lerp(checkpoint.leftGoal.position, checkpoint.rightGoal.position,
            checkpointPositionLerp);

        var moveCheckpoint = true;

        if (attackPlayer && target != null)
        {
            if (target.checkpoints.nextCheckpoint == checkpoint)
            {
                moveCheckpoint = false;

                var targetDir = target.rb.position - rb.position;
                var rbDir = car.currentInputDirection;
                rbDir.y = 0;

                var angle = Vector3.SignedAngle(rbDir, targetDir, Vector3.up);

                inputs.gasInput = 1;
                inputs.steerInput = angle < 0 ? -1 : 1;
            }
        }

        if (moveCheckpoint)
        {
            var targetDir = targetPos - rb.position;
            var rbDir = car.currentInputDirection;
            rbDir.y = 0;

            var angle = Vector3.SignedAngle(rbDir, targetDir, Vector3.up);

            inputs.gasInput = 1;
            inputs.steerInput = angle < 0 ? -1 : 1;
        }
    }
}
