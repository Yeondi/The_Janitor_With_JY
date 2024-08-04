using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public EnemyType Type { get; set; }

    //public Enemy(int health, int attackPower, float speed, Vector2 initialPosition, EnemyType type)
    //    : base(health, attackPower, speed, initialPosition)
    //{
    //    Type = type;
    //}

    public override void Initialize(int hp, int attackPower, float speed, Vector2 initialPosition, EnemyType type)
    {
        base.Initialize(hp, attackPower, speed, initialPosition,type);
        Type = type;
    }

    public override void Die()
    {
        Console.WriteLine("Enemy died. Type: " + Type);
    }
}
