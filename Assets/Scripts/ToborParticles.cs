using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToborParticles : MonoBehaviour
{
    public ParticleSystem[] tireSmokes;
    public TrailRenderer[] carTrails;
    public AnimationCurve trailLengthCurve;
    public ParticleSystem[] empParticles;

    public void Start()
    {
        StopDrift();
    }

    public void PlayEMP()
    {
        foreach (var emp in empParticles)
            emp.Play();
    }

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

    public void UpdateTrails(float speed)
    {
        var t = trailLengthCurve.Evaluate(speed);
        foreach (var trail in carTrails)
            trail.time = t;
    }
}
