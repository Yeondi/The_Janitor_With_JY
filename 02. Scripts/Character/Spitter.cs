using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpitterState
{
    Idle,
    Moving,
    Chasing,
    Attacking,
    Dead
}

public class Spitter : Character
{
    public EnemyType Type { get; set; }
    public SpitterState CurrentState { get; private set; } = SpitterState.Idle;

    public float DetectionRange = 10f;
    public float AttackRange = 2f; // ���� ����

    public GameObject proj_prefab;

    [SerializeField]
    private bool isAttacking = false;

    private float attackCooldown = 3f; // �⺻ ���� ��ٿ�
    private float lastAttackTime = -1f; // �⺻ ���� ������ ���� �ð�

    [SerializeField]
    private bool doNotFollow = false; // Ÿ�� ���� �� ������� ���� Ȱ��ȭ ���� (����׿�)

    private Animator animator;
    private Vector3 initialScale;

    private Vector2 savedDirection; // ���� ���� ���� ����

    public override void Initialize(int hp, int attackPower, float speed, Vector2 initialPosition, EnemyType type)
    {
        base.Initialize(hp, attackPower, speed, initialPosition,type);
        animator = GetComponent<Animator>();
        initialScale = transform.localScale; // �ʱ� ������ ����
        Type = type;
        StartCoroutine(RandomBehavior());
    }

    void Update()
    {
        if (CurrentState != SpitterState.Dead)
        {
            if (Target != null && !doNotFollow)
            {
                UpdateState();
                PerformStateAction();
            }
        }
    }

    IEnumerator RandomBehavior()
    {
        while (CurrentState != SpitterState.Dead)
        {
            if (Target == null || doNotFollow)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f));

                float action = Random.value;
                if (action < 0.33f) // 33% chance to stop
                {
                    CurrentState = SpitterState.Idle;
                    animator.SetBool("IsMoving", false);
                }
                else if (action < 0.66f) // 33% chance to move left
                {
                    CurrentState = SpitterState.Moving;
                    animator.SetBool("IsMoving", true);
                    float moveTime = Random.Range(1f, 3f);
                    Vector2 moveDirection = Vector2.left;

                    while (moveTime > 0)
                    {
                        Move(moveDirection);
                        moveTime -= Time.deltaTime;
                        yield return null;
                    }
                    animator.SetBool("IsMoving", false);
                }
                else // 33% chance to move right
                {
                    CurrentState = SpitterState.Moving;
                    animator.SetBool("IsMoving", true);
                    float moveTime = Random.Range(1f, 3f);
                    Vector2 moveDirection = Vector2.right;

                    while (moveTime > 0)
                    {
                        Move(moveDirection);
                        moveTime -= Time.deltaTime;
                        yield return null;
                    }
                    animator.SetBool("IsMoving", false);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void UpdateState()
    {
        if (isAttacking) return; // ���� ���� ���� ���¸� �������� ����

        float distanceToTarget = Vector3.Distance(Target.position, transform.position);

        if (Target != null)
        {
            if (distanceToTarget <= AttackRange)
            {
                CurrentState = SpitterState.Attacking;
            }
            else
            {
                CurrentState = SpitterState.Chasing;
            }
        }
        else
        {
            CurrentState = SpitterState.Idle;
            animator.SetBool("IsMoving", false);
        }
    }

    private void PerformStateAction()
    {
        if (Target != null && !doNotFollow)
        {
            float distanceToTarget = Vector3.Distance(Target.position, transform.position);
            if (distanceToTarget <= AttackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
            else if (distanceToTarget <= DetectionRange)
            {
                ChaseTarget();
            }
        }
        else
        {
            switch (CurrentState)
            {
                case SpitterState.Moving:
                    animator.SetBool("IsMoving", true);
                    break;
                case SpitterState.Idle:
                    HandleIdle();
                    break;
            }
        }
    }

    private void PerformAttack()
    {
        // �̹� ���� ���̶�� �� �̻� Ʈ���� �������� ����
        if (isAttacking) return;

        isAttacking = true;
        lastAttackTime = Time.time; // ������ ���� �ð��� ���� �ð����� ����
        savedDirection = (Target.position - transform.position).normalized;
        FaceDirection(savedDirection); // ���� �� Ÿ�� �������� ȸ��
        animator.SetTrigger("Attack");

        //LaunchProjectile(direction);
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(proj_prefab, transform.position, Quaternion.identity);
        SpitterProjectile projScript = projectile.GetComponent<SpitterProjectile>();
        if (projScript != null)
        {
            projScript.Initialize(savedDirection, AttackPower); // ����ü �ʱ�ȭ
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        if (CurrentState != SpitterState.Dead)
        {
            CurrentState = SpitterState.Idle;
        }
    }

    private void ChaseTarget()
    {
        if (Vector3.Distance(Target.position, transform.position) <= AttackRange)
        {
            // ���� ���� ���� ������ �̵��� ����
            HandleIdle();
            return;
        }

        Vector2 direction = (Target.position - transform.position).normalized;
        Move(direction); // Ÿ���� �Ѿư� ���� �̵� �ӵ� ����
        FaceDirection(direction);
        animator.SetBool("IsMoving", true);
    }

    private void HandleIdle()
    {
        animator.SetBool("IsMoving", false);
    }

    public override void Move(Vector2 direction)
    {
        // ���� ���� ���� �̵����� ����
        if (isAttacking) return;

        base.Move(direction * 0.05f); // �̵� �ӵ� ����
        animator.SetBool("IsMoving", true);
        FaceDirection(direction);
    }

    private void FaceDirection(Vector2 direction)
    {
        //if (!isAttacking) // ���� ���� �ƴ� ���� ���� ��ȯ
        //{
        if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // ������ �ٶ�
        }
        else if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // �������� �ٶ�
        }
        //}
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (Hp <= 0 && CurrentState != SpitterState.Dead)
        {
            CurrentState = SpitterState.Dead;
            Die();
        }
    }

    public override void Die()
    {
        Console.WriteLine("Enemy died. Type: " + Type);
        Debug.Log("Spider died.");
        animator.SetBool("IsDead", true);
        isAttacking = false;
    }

    public override void SetTarget(Transform target)
    {
        if (!doNotFollow)
        {
            Target = target;
        }
    }

    public void Destroy()
    {
        // ��ȸ�� �����̹Ƿ� ���� ������Ʈ Ǯ���� �ʿ䰡 ����.
        Destroy(gameObject);
    }

    #region Events


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetTarget(collision.transform);  // �÷��̾ Ÿ������ ����
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetTarget(null);  // �÷��̾ Ÿ������ ����
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TakeDamage(Target.GetComponent<Player>().AttackPower);  // ���Ͱ� ���ظ� ����
        }
    }

    #endregion
}
