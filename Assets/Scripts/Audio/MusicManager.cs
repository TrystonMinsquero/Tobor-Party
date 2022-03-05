using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    /// <summary>
    ///   <para>the name of the starting song you want to play at the start of the scene.</para>
    /// </summary>
    public string startingSongName;

    /// <summary>
    ///   <para>only change this in the prefab so it updates in all scenes.</para>
    /// </summary>
    public Song[] songs;

    private static List<Song> _songs = new List<Song>();

    public static Song currentSong;

    void Awake()
    {
        if (instance != null)
        {
            // Add songs not in current manager
            AddNewSongs(songs);
            
            if (startingSongName != currentSong.name)
            {
                Play(startingSongName);
            }
            
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            AddNewSongs(songs);
        
            if(currentSong == null || currentSong == new Song())
                Play(startingSongName);
            
            DontDestroyOnLoad(this.gameObject);
        }
        
    }

    // Adds new songs the the current instance
    private static void AddNewSongs(Song[] songs)
    {
        foreach (var newSong in songs)
        {
            if (_songs.Exists(song => newSong.name == song.name)) continue;
            newSong.source = instance.gameObject.AddComponent<AudioSource>();
            newSong.source.clip = newSong.clip;

            newSong.source.volume = newSong.volume;
            newSong.source.pitch = newSong.pitch;
            newSong.source.loop = newSong.loop;
            _songs.Add(newSong);
        }
    }
    

    public static void Play(string name)
    {
        Song s = null;
        foreach (var song in _songs)
        {
            if (song.name != name)
                song.source.Stop();
            else
                s = song;
        }
        if(s == null)
            Debug.LogWarning("Song was not in list");
        s?.source.Play();
        currentSong = s;
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}