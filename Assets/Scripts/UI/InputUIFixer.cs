using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputUIFixer : MonoBehaviour
{
    public InputSystemUIInputModule module;
    InputActionAsset asset;
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
        asset.devices = null;
        asset.bindingMask = null;
        module.actionsAsset = asset;
    }
}
