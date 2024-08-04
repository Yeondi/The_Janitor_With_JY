using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    public float Speed = 5f;
    private Vector2 direction;
    private int damage;

    public void Initialize(Vector2 direction, int damage)
    {
        this.direction = direction;
        this.damage = damage;
        Destroy(gameObject, 5f); // ����ü�� 5�� �Ŀ� �ڵ����� ��������� ����
    }

    void Update()
    {
        transform.Translate(direction * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        //else if (collision.CompareTag("Obstacles"))
        //{
        //    Destroy(gameObject); // ��ֹ��� �浹 �� ����ü �ı�
        //}
    }
}
