using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionNumberHandler : MonoBehaviour
{
    public Gradient placeGradient;
    public Sprite[] posNumImgs;
    private Image currentPosImg;
    public float addedGrowthSize;
    private int currentPos = 0;
    

    void Start()
    {
        currentPosImg = GetComponent<Image>();
    }

    public void UpdatePosition(int pos)
    {
        if (currentPos == pos) return;

        currentPos = pos;
        StartCoroutine(GrowthAnim(pos));
    }

    private IEnumerator GrowthAnim(int pos)
    {
        print("Growth called");
        transform.localScale = new Vector3(1, 1, 1);

        // play animation
        float lerpTime = 0, lerpDur = 0.15f;
        while (lerpTime < lerpDur)
        {
            float t = lerpTime / lerpDur;
            t = t * t * (3f - 2f * t);

            float y = Mathf.Lerp(0, addedGrowthSize, t);
            transform.localScale = new Vector3(1 + y, 1 + y * 0.8f, 1);
            
            lerpTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = new Vector3(1, 1 + addedGrowthSize, 1);
        
        currentPosImg.sprite = posNumImgs[pos - 1];
        currentPosImg.color = placeGradient.Evaluate((pos - 1f) / 7f);

        StartCoroutine(ShrinkAnim());
    }
    
    private IEnumerator ShrinkAnim()
    {
        print("Shrink called");
        transform.localScale = new Vector3(1, 1 + addedGrowthSize, 1);

        // play animation
        float lerpTime = 0, lerpDur = 0.15f;
        while (lerpTime < lerpDur)
        {
            float t = lerpTime / lerpDur;
            t = t * t * (3f - 2f * t);

            float y = Mathf.Lerp(addedGrowthSize, 0, t);
            transform.localScale = new Vector3(1 + y, 1 + y * 0.8f, 1);
            
            lerpTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = new Vector3(1, 1, 1);
    }
}
