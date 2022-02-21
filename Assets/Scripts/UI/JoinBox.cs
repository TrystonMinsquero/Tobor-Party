using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JoinBox : MonoBehaviour
{
    [Header("Data")]
    public int slot;
    public bool hasPlayer { get; private set; }
    public bool isReady { get; private set; }

    [Header("UI Draggables")]
    public Canvas joined;
    public Canvas empty;
    public TMP_Text readyUpText;

    public ToborPreview preview;
    public UIController _controller;

    public void AddPlayer(Player player)
    {
        empty.enabled = false;
        joined.enabled = true;
        hasPlayer = true;
        UnReady();

        preview.Enable();
        _controller = player.GetComponent<UIController>();
    }

    private void Update()
    {
        if (_controller)
        {
            preview.renderer.Rotate(-_controller.Rotate * 200 * Time.deltaTime);

           if(_controller.ChangeSkin != 0) 
               preview.renderer.ChangeSkin(_controller.ChangeSkin > 0);
           
           if(_controller.Ready) 
               ReadyUp();
           if (_controller.Leave)
           {
               if (isReady)
               {
                   UnReady();
               }
           }
            

        }
    }

    public void RemovePlayer(Player player)
    {
        joined.enabled = false;
        empty.enabled = true;
        hasPlayer = false;
    }

    public void ReadyUp()
    {
        isReady = true;
        readyUpText.enabled = true;
    }

    public void UnReady()
    {
        isReady = false;
        readyUpText.enabled = false;
    }
    
}
