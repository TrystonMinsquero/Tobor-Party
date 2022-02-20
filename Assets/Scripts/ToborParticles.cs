using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToborParticles : MonoBehaviour
{
    public ParticleSystem[] tireSmokes;

    public void StartDrift()
    {
        foreach (var tireSmoke in tireSmokes)
            if (!tireSmoke.isPlaying)
                tireSmoke.Play();
    }

    public void StopDrift()
    {
        foreach (var tireSmoke in tireSmokes)
            if (tireSmoke.isPlaying)
                tireSmoke.Stop();
    }
}
