using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : Item
{
    public int uses = 1;

    public float boostTime = 0.5f;
    public float itemCooldown = 0.8f;

    private Car car;

    // Boost temporarily
    public override void Activate(Car car)
    {
        this.car = car;

        car.Boost(boostTime);
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        car.UsingItem = true;
        yield return new WaitForSeconds(itemCooldown);
        car.UsingItem = false;

        uses--;
        if (uses <= 0)
            car.DiscardItem();
    }
}
