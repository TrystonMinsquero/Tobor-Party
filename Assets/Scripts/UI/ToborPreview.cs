using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToborPreview : MonoBehaviour
{
    public LayerMask layer;
    public ToborRenderer renderer;
    public RawImage outputImage;

    // Start is called before the first frame update
    void Start()
    {
        renderer = Instantiate(renderer);
        renderer.Initialize(layer, outputImage);
        renderer.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (renderer.Enabled)
            renderer.Rotate(0, Time.deltaTime * 20f);
    }

    public void Enable()
    {
        renderer.Enable();
    }
}
