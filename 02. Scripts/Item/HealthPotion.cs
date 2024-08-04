using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Item
{
    public int HealAmount { get; set; }

    public override void Use(Character target)
    {
        Console.WriteLine("Health Potion used. Healing: " + HealAmount);
        target.Hp += HealAmount;
    }
}