using UnityEngine.Audio;
using System;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioMixerGroup sfxGroup;
    public Sound[] sounds;

    void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;


        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = sfxGroup;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public static void Play (string name)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name);

        s.source.Play();
    }

}