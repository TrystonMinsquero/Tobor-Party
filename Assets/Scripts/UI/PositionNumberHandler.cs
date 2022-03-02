using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionNumberHandler : MonoBehaviour
{
    public Gradient placeGradient;
    public Sprite[] posNumImgs;
    private Image currentPosImg;
    

    void Start()
    {
        currentPosImg = this.GetComponent<Image>();
    }

    public void UpdatePosition(int pos)
    {
        currentPosImg.sprite = posNumImgs[pos - 1];
        currentPosImg.color = placeGradient.Evaluate((pos - 1f) / 7f);
    }
}
