using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToborParticles : MonoBehaviour
{
    public ParticleSystem[] driftSparks;
    public ParticleSystem[] tireSmokes;
    public TrailRenderer[] carTrails;
    public AnimationCurve trailLengthCurve;
    public ParticleSystem[] empParticles;
    public ParticleSystem[] speedLineParticles;
    public MeshRenderer bodyMaterialRenderer;
    public Material starMaterial;
    private Material defaultMaterial;

    public Gradient swiftBoostColor;
    public AnimationCurve swiftParticleRateCurve;

    public void Start()
    {
        defaultMaterial = bodyMaterialRenderer.sharedMaterials[0];
        StopDrift();
    }

    public void ResetTexture()
    {
        bodyMaterialRenderer.sharedMaterials[0] = defaultMaterial;
    }

    public void StarTexture()
    {
        bodyMaterialRenderer.sharedMaterials[0] = starMaterial;
    }

    public void PlayEMP()
    {
        foreach (var emp in empParticles)
            emp.Play();
    }

    public void PlaySpeedLines()
    {
        foreach (var speedLines in speedLineParticles)
            if (!speedLines.isPlaying)
                speedLines.Play();
    }

    public void StopSpeedLines()
    {
        foreach (var speedLines in speedLineParticles)
            if (speedLines.isPlaying)
                speedLines.Stop();
    }

    public void StartDrift()
    {
        foreach (var tireSmoke in tireSmokes)
            if (!tireSmoke.isPlaying)
                tireSmoke.Play();

        foreach (var spark in driftSparks)
            if (!spark.isPlaying)
                spark.Play();
    }

    public void StopDrift()
    {
        foreach (var tireSmoke in tireSmokes)
            if (tireSmoke.isPlaying)
                tireSmoke.Stop();

        foreach (var spark in driftSparks)
            if (spark.isPlaying)
                spark.Stop();
    }

    public void UpdateTrails(float speed)
    {
        var t = trailLengthCurve.Evaluate(speed);
        foreach (var trail in carTrails)
            trail.time = t;
    }

    public void UpdateSwiftBoostColor(float swiftPercentage)
    {
        foreach (var spark in driftSparks)
        {
            var main = spark.main;
            main.startColor = swiftBoostColor.Evaluate(swiftPercentage);

            var emm = spark.emission;
            emm.rateOverTimeMultiplier = swiftParticleRateCurve.Evaluate(swiftPercentage);
        }
    }
}
