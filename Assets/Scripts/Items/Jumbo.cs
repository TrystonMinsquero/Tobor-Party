using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumbo : Item
{
    public float scaleFactor = 2f;
    public float scaleLerpTime = 0.5f;
    public float scaleTime = 10;
    private Car car;
    
    public override void Activate(Car car)
    {
        this.car = car;
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        car.UsingItem = true;

        float t = 0;
        while (t < scaleLerpTime)
        {
            car.Scale = Mathf.Lerp(1, scaleFactor, t / scaleLerpTime);
            t += Time.deltaTime;
            yield return null;
        }
        car.Scale = scaleFactor;

        yield return new WaitForSeconds(scaleTime);
        t = 0;
        while (t < scaleLerpTime)
        {
            car.Scale = Mathf.Lerp(scaleFactor, 1, t / scaleLerpTime);
            t += Time.deltaTime;
            yield return null;
        }
        car.Scale = 1;

        car.UsingItem = false;
        car.DiscardItem();
    }

}
