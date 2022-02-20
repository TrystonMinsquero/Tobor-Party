using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToborRenderer : MonoBehaviour
{
    public bool Enabled => tobor.gameObject.activeSelf;

    public Transform tobor;
    public Camera camera;
    public int width = 256;

    public RenderTexture texture;

    public Vector2 rotation = new Vector2(15f, 215f);

    void SetLayer(Transform t, LayerMask layer)
    {
        t.gameObject.layer = layer;

        foreach (Transform obj in t)
        {
            SetLayer(obj, layer);
        }
    }

    public int maskToLayer(LayerMask myLayer)
    {
        int layerNumber = 0;
        int layer = myLayer.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }

    public void Initialize(LayerMask layer, RawImage output)
    {
        int l = maskToLayer(layer);
        SetLayer(tobor, l);

        camera.cullingMask = layer;

        texture = new RenderTexture(width, width, 0);
        camera.targetTexture = texture;

        output.texture = texture;
    }

    public void Rotate(float x, float y)
    {
        rotation += new Vector2(x, y);
        rotation.x = Mathf.Clamp(x, -30f, 30f);
        tobor.rotation = Quaternion.Euler(0, rotation.y, 0) * Quaternion.Euler(rotation.x, 0, 0);
    }

    public void Enable()
    {
        tobor.gameObject.SetActive(true);
    }

    public void Disable()
    {
        tobor.gameObject.SetActive(false);
    }
}
