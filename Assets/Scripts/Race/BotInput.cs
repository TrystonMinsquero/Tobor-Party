using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInput : MonoBehaviour
{
    public void UpdateInput(ref FrameInputs inputs, CheckpointUser cpUser)
    {
        inputs.gasInput = 1;
        inputs.steerInput = 1;
    }
}
