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
        // ������, ������ ���� ���� ��� ĳ���Ϳ��� ���ظ� �شٰ� ����
    }
}
