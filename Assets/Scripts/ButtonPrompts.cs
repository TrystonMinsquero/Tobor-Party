using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public enum Device
{
    XInput,
    Keyboard,
    PlayStation
}

[Serializable]
public class DevicePrompt
{
    public Device device;
    public Sprite promptSprite;
}

[Serializable]
public class ButtonPrompt
{
    public string actionName;
    public DevicePrompt[] devicePrompts;
}

[Serializable]
[CreateAssetMenu(fileName = "ButtonPrompts")]
public class ButtonPrompts : ScriptableObject
{
    public ButtonPrompt[] buttonPrompts;

    public DevicePrompt[] GetDevicePrompts(string actionName)
    {
        foreach (var buttonPrompt in buttonPrompts)
        {
            if (buttonPrompt.actionName == actionName)
                return buttonPrompt.devicePrompts;
        }
        Debug.LogWarning($"{actionName} action name not found in buttonPrompts");
        return null;
    }
}