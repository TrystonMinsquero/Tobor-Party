using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToborRenderer : MonoBehaviour
{
    public bool Enabled => tobor.gameObject.activeSelf;

    public Transform tobor;
    public new Camera camera;
    public int width = 256;
    public int height = 256;

    public RenderTexture texture;
    public PlayerSkin skin;

    public Vector2 rotation = new Vector2(15f, 215f);

    void Start()
    {
        skin = tobor.GetComponent<PlayerSkin>();
    }

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

    public void ResetLayer()
    {
        int l = maskToLayer(layer);
        SetLayer(tobor, l);

        camera.cullingMask = layer;
    }

    private LayerMask layer;
    public void Initialize(LayerMask layer, RawImage output)
    {
        this.layer = layer;
        ResetLayer();

        texture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        camera.targetTexture = texture;

        output.texture = texture;
    }

    public void Rotate(float y, float x = 0)
    {
        rotation += new Vector2(x, y);
        rotation.x = Mathf.Clamp(x, -30f, 30f);
        tobor.rotation = Quaternion.Euler(0, rotation.y, 0) * Quaternion.Euler(rotation.x, 0, 0);
    }
    public void ChangeSkin(bool right)
    {
        if (right) skin.NextSkin();
        else skin.PrevSkin();

        ResetLayer();
    }

    public void UpdateSkin(int index)
    {
        if (skin == null)
            skin = tobor.GetComponent<PlayerSkin>();
        skin.SetIndex(index);
        ResetLayer();
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
