using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public bool isEnabled = true;
    public float delayTime = 5f;
    public GameObject visual;

    public float verticalWavesPerSecond = 5;
    public float floatDistance = 0.4f;
    public float rotationsPerSecond = 10;
    public float verticalOffset = 0.2f;

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

    void Update()
    {
        visual.transform.localPosition = Vector3.up * (verticalOffset + floatDistance * Mathf.Sin(2 * Mathf.PI * Time.time * verticalWavesPerSecond));
        visual.transform.localRotation = Quaternion.Euler(0, (Time.time * 360 * rotationsPerSecond) % 360, 0);
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
