﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

// modified from code source: https://gist.github.com/emredesu/af597de14a4377e1ecf96b6f7b6cc506

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;

    private Vector2 m_NextScrollPosition = Vector2.up;
    private InputSystemUIInputModule _module;
    
    void OnEnable() {
        if (m_ScrollRect) {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
    }
    void Awake() {
        m_ScrollRect = GetComponent<ScrollRect>();
        _module = FindObjectOfType<InputSystemUIInputModule>();
    }
    void Start() {
        if (m_ScrollRect) {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        ScrollToSelected(true);
    }
    void Update() {
        // If we are on mobile, do not do anything.
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            return;
        }

        // Scroll via input.
        InputScroll();
        if (!mouseOver) {
            // Lerp scrolling code.
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
        }
        else {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }

#nullable enable
    void InputScroll() {
        if (m_Selectables.Count > 0) {
            Keyboard? currentKeyboard = Keyboard.current;
            Gamepad? currentGamepad = Gamepad.current;

            if (currentKeyboard != null) {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame || 
                    _module && _module.move.ToInputAction().ReadValue<Vector2>().magnitude > .1f) {
                    ScrollToSelected(false);
                }
            }

            if (currentGamepad != null) {
                if (Gamepad.current.dpad.up.wasPressedThisFrame || Gamepad.current.dpad.down.wasPressedThisFrame || 
                    _module && _module.move.ToInputAction().ReadValue<Vector2>().magnitude > .1f) {
                    ScrollToSelected(false);
                }
            }
        }
    }
#nullable disable
    void ScrollToSelected(bool quickScroll) {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement) {
            selectedIndex = m_Selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1) {
            if (quickScroll) {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
            else {
                m_NextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData) {
        mouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData) {
        mouseOver = false;
        ScrollToSelected(false);
    }
}