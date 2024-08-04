using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IMovable, IAttackable
{
    public Vector2 Position { get; set; }

    [Tooltip("현재 체력")]
    [SerializeField]
    private int hp = 100;
    [Tooltip("기본 이동 속도")]
    [SerializeField]
    private float movementSpeed = 1.0f;
    [Tooltip("달리기 속도")]
    [SerializeField]
    private float sprintSpeed = 3.0f; // 이동속도 * 3

    [SerializeField]
    private int attackPower = 1;
    [SerializeField]
    private Transform target;

    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    public float MovementSpeed
    {
        get { return movementSpeed; }
        set { movementSpeed = value; }
    }
    public float SprintSpeed
    {
        get { return sprintSpeed; }
        set { sprintSpeed = value; }
    }
    public int AttackPower
    {
        get { return attackPower; }
        set { attackPower = value; }
    }
    public Transform Target
    {
        get { return target; }
        set {  target = value; }
    }

    // Initialize for Player
    public virtual void Initialize(int hp, int attackPower, float movementSpeed, float sprintSpeed, Vector2 initialPosition)
    {
        Hp = hp;
        AttackPower = attackPower;
        MovementSpeed = movementSpeed;
        SprintSpeed = sprintSpeed;
        Position = initialPosition;
    }

    // Initialize for Enemy type
    public virtual void Initialize(int hp, int attackPower, float movementSpeed, Vector2 initialPosition, EnemyType type)
    {
        Hp = hp;
        AttackPower = attackPower;
        MovementSpeed = movementSpeed;
        Position = initialPosition;
    }

    public virtual void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction * MovementSpeed * Time.deltaTime;
    }

    // ĳ���� ���� �޼���
    public virtual void Attack(Character target)
    {
        // �⺻ ���� ��Ŀ����
        Debug.Log("Attacks with basic power of " + AttackPower);
    }

    // ĳ���Ͱ� ���ظ� �Դ� �޼���
    public virtual void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Die();
        }
    }

    // ĳ���� ��� �޼���
    public virtual void Die()
    {
        Console.WriteLine("Character died.");
    }

    public virtual void SetTarget(Transform target)
    {
        Target = target;
        Debug.Log("Target set from external method.");
    }

}
