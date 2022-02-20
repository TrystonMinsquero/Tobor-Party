using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public bool isEnabled = true;
    public float delayTime = 5f;
    public GameObject visual;

    Item GetItem(float position)
    {
        return null;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!isEnabled)
            return;

        if (collider.transform.TryGetComponent<Car>(out var car))
        {
            car.AddItem(null);

            StartCoroutine(UseItemDelay());
        }
    }

    IEnumerator UseItemDelay()
    {
        isEnabled = false;
        visual.SetActive(false);
        yield return new WaitForSeconds(delayTime);
        visual.SetActive(true);
        isEnabled = true;
    }
}
