using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform leftGoal;
    public Transform rightGoal;
    public int index;
    public int nameIndex;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.TryGetComponent<CheckpointUser>(out var user))
        {
            user.CheckpointReached(this);
        }
    }
}
