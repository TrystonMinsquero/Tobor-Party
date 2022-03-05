using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]

public class Song
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f,3f)]
    public float pitch = 1f;

    public bool loop = true;

    [HideInInspector]
    public AudioSource source;
}