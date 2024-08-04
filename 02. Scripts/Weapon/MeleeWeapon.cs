using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public override void Use(Character target)
    {
        Debug.Log("Melee attack! Damage: " + Damage);
        target.TakeDamage(Damage);
    }
}