using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Item
{
    public int ExplosionDamage { get; set; }

    public override void Use(Character target)
    {
        Console.WriteLine("Bomb explodes! Damage: " + ExplosionDamage);
        // 예제로, 폭발은 범위 내의 모든 캐릭터에게 피해를 준다고 가정
    }
}
