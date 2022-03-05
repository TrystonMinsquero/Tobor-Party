using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    // Maybe switch this to canvas groups?
    public Canvas startMenu;
    public Canvas playMenu;
    public Canvas settingsMenu;
    public Canvas credits;

    public Button initStartButton;
    public Button initPlayButton;
    public Button initSettingsButton;
    public Button initCreditsButton;

    private void Start()
    {
        ShowStartMenu();
    }

    public void ShowStartMenu()
    {
        settingsMenu.enabled = false;
        playMenu.enabled = false;
        credits.enabled = false;
        startMenu.enabled = true;
        initStartButton.Select();
    }

    public void ShowPlayMenu()
    {
        settingsMenu.enabled = false;
        credits.enabled = false;
        startMenu.enabled = false;
        playMenu.enabled = true;
        initPlayButton.Select();
    }

    public void ShowSettings()
    {
        credits.enabled = false;
        startMenu.enabled = false;
        playMenu.enabled = false;
        settingsMenu.enabled = true;
        initSettingsButton.Select();
    }

    public void ShowCredits()
    {
        settingsMenu.enabled = false;
        startMenu.enabled = false;
        playMenu.enabled = false;
        credits.enabled = true;
        initCreditsButton.Select();
    }

    public void Quit()
    {
        // todo: add a are you sure message
        Application.Quit();
    }
    
}
