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
        float totalWeight = 0;
        foreach (Item item in items)
            totalWeight += Mathf.Lerp(item.probability.x, item.probability.y, position);

        double r = Random.value * totalWeight;
        foreach (Item item in items)
        {
            var weight = Mathf.Lerp(item.probability.x, item.probability.y, position);
            if (weight >= r)
            {
                var clone = Instantiate(item);
                return clone;
            }

            r -= weight;
        }
        return null;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!isEnabled)
            return;

        if (collider.transform.TryGetComponent<Car>(out var car))
        {
            float pos = RaceManager.instance.cars.IndexOf(car.GetComponent<CheckpointUser>());
            pos /= RaceManager.instance.cars.Count;

            car.AddItem(GetItem(pos));

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
