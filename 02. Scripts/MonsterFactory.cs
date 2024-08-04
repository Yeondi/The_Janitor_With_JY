using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFactory : MonoBehaviour
{
    public GameObject spiderPrefab;   // Spider ÇÁ¸®ÆÕ
    public GameObject ghoulPrefab;    // ±¸¿ï ÇÁ¸®ÆÕ
    public GameObject spitterPrefab;  // ½ºÇÇÅÍ ÇÁ¸®ÆÕ

    private void Start()
    {
        CreateMonster(EnemyType.Boss, new Vector2(1f, 1.92f), 100, 10, 30);
        CreateMonster(EnemyType.NormalGhoul, new Vector2(3.98f, 1.92f), 100, 10, 30);
        CreateMonster(EnemyType.NormalSpitter, new Vector2(8.98f, 1.92f), 100, 10, 30);
    }

    public GameObject CreateMonster(EnemyType type, Vector2 position, int health, int attackPower, float speed)
    {
        GameObject monster = null;

        switch (type)
        {
            case EnemyType.Boss:
                monster = Instantiate(spiderPrefab, position, Quaternion.identity);
                var spider = monster.GetComponent<Spider>();
                if (spider != null)
                {
                    spider.Initialize(health, attackPower, speed, position, type);
                }
                break;

            case EnemyType.NormalGhoul:
                monster = Instantiate(ghoulPrefab, position, Quaternion.identity);
                var ghoul = monster.GetComponent<Ghoul>();
                if (ghoul != null)
                {
                    ghoul.Initialize(health, attackPower, speed, position, type);
                }
                break;

            case EnemyType.NormalSpitter:
                monster = Instantiate(spitterPrefab, position, Quaternion.identity);
                var spitter = monster.GetComponent<Spitter>();
                if (spitter != null)
                {
                    spitter.Initialize(health, attackPower, speed, position, type);
                }
                break;
        }

        return monster;
    }
}
