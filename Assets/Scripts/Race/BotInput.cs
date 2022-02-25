using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BotState
{
    public static BotState Default => new DefaultState();
    public static BotState Turn => new TurnState();
    public static BotState Drift => new DriftState();


    public abstract BotState Run(BotInput bot, ref FrameInputs inputs, 
        Vector3 pos, float angle);
}

public class DefaultState : BotState
{
    public override BotState Run(BotInput bot, ref FrameInputs inputs, Vector3 pos, float angle)
    {
        inputs.gasInput = 1;
        inputs.steerInput = 0;
        inputs.drift = false;

        if (Mathf.Abs(angle) > bot.turnThreshold)
            return Turn;

        return this;
    }
}

public class TurnState : BotState
{
    bool prevItem = false;

    public override BotState Run(BotInput bot, ref FrameInputs inputs, Vector3 pos, float angle)
    {
        inputs.useItem = prevItem;
        prevItem = !prevItem;

        inputs.gasInput = 1;
        inputs.drift = false;

        if (Mathf.Abs(angle) < bot.turnEndThreshold)
            return Default;

        inputs.steerInput = angle < 0 ? -1 : 1;

        return this;
    }
}

public class DriftState : BotState
{
    public override BotState Run(BotInput bot, ref FrameInputs inputs, Vector3 pos, float angle)
    {
        inputs.gasInput = 1;
        inputs.drift = true;

        if (Mathf.Abs(angle) < bot.driftEndThreshold)
            return Default;

        inputs.steerInput = angle < 0 ? -1 : 1;

        return this;
    }
}

public class BotInput : MonoBehaviour
{
    public bool attackPlayer = false;

    public BotState state;

    public float turnThreshold = 20f;
    public float turnEndThreshold = 5f;

    public float driftEndThreshold = 10f;
    public float driftThreshold = 70f;

    public float checkpointPositionLerp = 0.5f;

    private Car target;

    void Start()
    {
        state = BotState.Default;
        //checkpointPositionLerp = Random.value;

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

            state = state.Run(this, ref inputs, targetDir, angle);
        }
    }
}
