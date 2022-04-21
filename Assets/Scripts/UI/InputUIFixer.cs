using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputUIFixer : MonoBehaviour
{
    public InputSystemUIInputModule module;
    InputActionAsset asset;

    private bool _activate = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (!module)
            module = GetComponent<InputSystemUIInputModule>();
        asset = module.actionsAsset;
    }

    // Update is called once per frame
    void Update()
    {
        if (_activate || Arcade.IsRunningInArcadeMode())
            return;
        asset.devices = null;
        asset.bindingMask = null;
        module.actionsAsset = asset;
    }

    public void SetActive(bool val)
    {
        _activate = val;
    }
}
