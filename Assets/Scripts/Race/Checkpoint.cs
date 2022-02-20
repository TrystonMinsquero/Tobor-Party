using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;
    public int nameIndex;

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log(collider.transform);
        if (collider.transform.TryGetComponent<CheckpointUser>(out var user))
        {
            user.CheckpointReached(this);
        }
    }
}
