using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Menu
{
    public string name;
    public GameObject root;
    public Button initButton;

    public void SetActive(bool enable)
    {
        root?.SetActive(enable);
        if(enable)
            initButton?.Select();
    }
}

public class MainMenuUI : MonoBehaviour
{
    public List<Menu> menus;
    private string currentMenu
    {
        set
        {
            var activeMenu = menus.Find(menu => menu.name == value);
            if (activeMenu == new Menu())
            {
                Debug.LogWarning("Menu name not found");
                return;
            }
            foreach (var menu in menus)
                menu.SetActive(false);
            activeMenu.SetActive(true);
        }
    }

    private void Start()
    {
        currentMenu = "Start";
    }

    public void SetMenu(string menuName)
    {
        currentMenu = menuName;
    }

    public void Quit()
    {
        // todo: add a are you sure message
        Application.Quit();
    }
    
}
