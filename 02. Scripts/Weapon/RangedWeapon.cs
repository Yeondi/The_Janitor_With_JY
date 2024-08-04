using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    public override void Use(Character target)
    {
        Debug.Log("Ranged attack! Damage: " + Damage);
        target.TakeDamage(Damage);
    }
}
