using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinBox : MonoBehaviour
{
    public bool hasPlayer { get; private set; }
    public bool isReady { get; private set; }

    [Header("UI Draggables")]
    public Canvas joined;
    public Canvas empty;
    public TMP_Text readyText;
    public GameObject readyUpText;
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
        preview.renderer.UpdateSkin(_player.skinIndex);
        
        if (_controller)
        {
            _controller.Ready += ReadyUp;
            _controller.Leave += BackOut;
            _controller.ChangeSkin += ChangeSkin;
        }
    }

    private void Update()
    {
        if (_controller)
        {
            preview.renderer.Rotate(-_controller.Rotate * 400 * Time.deltaTime);
        }
    }

    public void BackOut()
    {
        if(isReady)
            UnReady();
        else if (PlayerManager.playerCount == 1)
        {
            PlayerManager.ClearAndDestroy();
            SceneManager.LoadScene("MainMenu");
        }
        else
            RemovePlayer(_player);
    }

    public void RemovePlayer(Player player)
    {
        if (_controller)
        {
            _controller.Ready -= ReadyUp;
            _controller.Leave -= BackOut;
            _controller.ChangeSkin -= ChangeSkin;
        }
        if(joined)
            joined.enabled = false;
        if(empty)
            empty.enabled = true;
        if(readyUpText)
            readyUpText.SetActive(false);
        hasPlayer = false;
        _player = null;
        PlayerManager.RemovePlayer(player);
    }

    public void ReadyUp()
    {
        isReady = true;
        if(readyText)
            readyText.text = "Ready!";
        if(readyUpText)
            readyUpText.SetActive(false);
    }

    public void UnReady()
    {
        isReady = false;
        if(readyText)
            readyText.text = "";
        if(readyUpText)
            readyUpText.SetActive(true);
    }

    private void ChangeSkin(float input)
    {
        if(input != 0)
            preview.renderer.ChangeSkin(input > 0);
        _player.skinIndex = preview.renderer.skin.Index;
    }
    
}
