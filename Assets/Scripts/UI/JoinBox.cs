using System;
using UnityEngine;

public class JoinBox : MonoBehaviour
{
    [Header("Data")]
    public int slot;
    public bool hasPlayer;

    [Header("UI Draggables")]
    public Canvas joined;
    public Canvas empty;

    public ToborPreview preview;
    public UIController _controller;

    public void AddPlayer(Player player)
    {
        empty.enabled = false;
        joined.enabled = true;
        hasPlayer = true;
        
        preview.Enable();
        _controller = player.GetComponent<UIController>();
    }

    private void Update()
    {
        if(_controller)
            preview.renderer.Rotate(-_controller.Rotate * 3);
    }

    public void RemovePlayer(Player player)
    {
        joined.enabled = false;
        empty.enabled = true;
        hasPlayer = false;
    }
    
}
