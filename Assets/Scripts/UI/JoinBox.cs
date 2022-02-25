using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JoinBox : MonoBehaviour
{
    public bool hasPlayer { get; private set; }
    public bool isReady { get; private set; }

    [Header("UI Draggables")]
    public Canvas joined;
    public Canvas empty;
    public TMP_Text readyUpText;
    public ToborPreview preview;

    private UIController _controller;
    private Player _player;

    public void AddPlayer(Player player)
    {
        empty.enabled = false;
        joined.enabled = true;
        hasPlayer = true;
        _player = player;
        UnReady();

        preview.Enable();
        _controller = player.GetComponent<UIController>();
        _controller.controls.UI.Ready.Enable();
        
        if (_controller)
        {
            _controller.Ready += ReadyUp;
            _controller.Leave += BackOut;
        }
    }

    private void Update()
    {
        if (_controller)
        {
            preview.renderer.Rotate(-_controller.Rotate * 400 * Time.deltaTime);

           if(_controller.ChangeSkin != 0) 
               preview.renderer.ChangeSkin(_controller.ChangeSkin > 0);
           //
           // if(_controller.Ready) 
           //     ReadyUp();
           // if (_controller.Leave)
           // {
           //     if (isReady)
           //     {
           //         UnReady();
           //     }
           //     else
           //     {
           //         PlayerManager.RemovePlayer(player);
           //     }
           // }
           //  

        }
    }

    public void BackOut()
    {
        if(isReady)
            UnReady();
        else
            RemovePlayer(_player);
    }

    public void RemovePlayer(Player player)
    {
        if (_controller)
        {
            _controller.Ready -= ReadyUp;
            _controller.Ready -= BackOut;
        }
        if(joined)
            joined.enabled = false;
        if(empty)
            empty.enabled = true;
        hasPlayer = false;
        _player = null;
        PlayerManager.RemovePlayer(player);
    }

    public void ReadyUp()
    {
        isReady = true;
        readyUpText.text = "Ready!";
    }

    public void UnReady()
    {
        isReady = false;
        readyUpText.text = "";
    }
    
}
