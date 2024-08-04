using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GhoulState
{
    Idle,
    Moving,
    Chasing,
    Attacking,
    Dead
}

public class Ghoul : Character
{
    public EnemyType Type { get; set; }
    public GhoulState CurrentState { get; private set; } = GhoulState.Idle;

    public float DetectionRange = 10f;
    public float AttackRange = 2f; // ���� ����

    [SerializeField]
    private bool isAttacking = false;

    private float attackCooldown = 1f; // �⺻ ���� ��ٿ�
    private float lastAttackTime = -1f; // �⺻ ���� ������ ���� �ð�

    [SerializeField]
    private bool doNotFollow = false; // Ÿ�� ���� �� ������� ���� Ȱ��ȭ ���� (����׿�)

    private Animator animator;
    private Vector3 initialScale;

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
        if (CurrentState != GhoulState.Dead)
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
        while (CurrentState != GhoulState.Dead)
        {
            if (Target == null || doNotFollow)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f));

                float action = Random.value;
                if (action < 0.33f) // 33% chance to stop
                {
                    CurrentState = GhoulState.Idle;
                    animator.SetBool("IsMoving", false);
                }
                else if (action < 0.66f) // 33% chance to move left
                {
                    CurrentState = GhoulState.Moving;
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
                    CurrentState = GhoulState.Moving;
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
        if (distanceToTarget <= AttackRange)
        {
            CurrentState = GhoulState.Attacking;
        }
        else if (distanceToTarget <= DetectionRange)
        {
            CurrentState = GhoulState.Chasing;
        }
        else
        {
            CurrentState = GhoulState.Idle;
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
                case GhoulState.Moving:
                    animator.SetBool("IsMoving", true);
                    break;
                case GhoulState.Idle:
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
        Vector2 direction = (Target.position - transform.position).normalized;
        FaceDirection(direction); // ���� �� Ÿ�� �������� ȸ��
        animator.SetTrigger("Attack");
    }

    public void EndAttack()
    {
        isAttacking = false;
        if (CurrentState != GhoulState.Dead)
        {
            CurrentState = GhoulState.Idle;
        }
    }

    private void ChaseTarget()
    {
        Debug.Log("Spider is chasing the target.");
        Vector2 direction = (Target.position - transform.position).normalized;
        Move(direction); // Ÿ���� �Ѿư� ���� �̵� �ӵ� ����
        FaceDirection(direction);
        animator.SetBool("IsMoving", true);
    }

    private void HandleIdle()
    {
        Debug.Log("Spider is idling.");
        animator.SetBool("IsMoving", false);
    }

    public override void Move(Vector2 direction)
    {
        // ���� ���� ���� �̵����� ����
        if (isAttacking) return;

        Debug.Log("Move�Լ� ���� ���� ���� : " + CurrentState.ToString());
        Debug.Log("Move�Լ� ���� ���� �� : isAttacking " + isAttacking.ToString());
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
        if (Hp <= 0 && CurrentState != GhoulState.Dead)
        {
            CurrentState = GhoulState.Dead;
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
