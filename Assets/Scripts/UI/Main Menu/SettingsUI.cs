using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] _resolutions;

    public void Start()
    {
        SetupResolutions();
    }

    private void SetupResolutions()
    {
        _resolutions = Screen.resolutions;
        Array.Reverse(_resolutions);
        List<string> options = new List<string>();

        foreach (var resolution in _resolutions)
            options.Add($"{resolution.width} x {resolution.height}");

        options = options.Distinct().ToList();

        int currentIndex = _resolutions.ToList().FindIndex(res => Screen.currentResolution.width == res.width && 
                                                                  Screen.currentResolution.height == res.height);
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    public void SetMasterVolume(float val)
    {
        audioMixer.SetFloat("MasterVolume", val);
    }
    
    public void SetMusicVolume(float val)
    {
        audioMixer.SetFloat("MusicVolume", val);
    }
    
    public void SetSfxVolume(float val)
    {
        audioMixer.SetFloat("SfxVolume", val);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        var res = _resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
