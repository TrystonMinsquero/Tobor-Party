using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInput : MonoBehaviour
{
    public void Update(ref FrameInputs inputs, CheckpointUser cpUser)
    {
        inputs.gasInput = 1;
        inputs.steerInput = 1;
    }
}
