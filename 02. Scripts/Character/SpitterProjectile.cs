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
        Destroy(gameObject, 5f); // 투사체가 5초 후에 자동으로 사라지도록 설정
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
        //    Destroy(gameObject); // 장애물에 충돌 시 투사체 파괴
        //}
    }
}
