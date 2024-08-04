using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpiderState
{
    Idle,
    Moving,
    Chasing,
    Attacking,
    JumpAttack,
    SpitAttack,
    Dead
}

public class Spider : Character
{
    public EnemyType Type { get; set; }
    public SpiderState CurrentState { get; private set; } = SpiderState.Idle;

    public float DetectionRange = 10f;
    public float AttackRange = 1.5f; // 공격 범위
    public float JumpAttackRange = 7f;  // 점프 공격 인식 범위
    public float SpitAttackRange = 10f; // 침뱉기 공격 인식 범위

    private float jumpAttackCooldown = 10f;
    private float spitAttackCooldown = 8f;
    private float lastJumpAttackTime = -10f;
    private float lastSpitAttackTime = -8f;

    [SerializeField]
    private bool isAttacking = false;

    [SerializeField]
    private bool doNotFollow = false; // 타겟 지정 시 따라오기 금지 활성화 여부 (디버그용)
    public LayerMask ObstacleLayer; // 장애물 레이어
    public float checkDistance = 1f; // 장애물 감지 범위

    private Animator animator;
    private Vector3 initialScale;

    public override void Initialize(int hp, int attackPower, float speed, Vector2 initialPosition, EnemyType type)
    {
        base.Initialize(hp, attackPower, speed, initialPosition, type);
        animator = GetComponent<Animator>();
        initialScale = transform.localScale; // 초기 스케일 저장
        Type = type;
        StartCoroutine(RandomBehavior());
    }

    void Update()
    {
        if (CurrentState != SpiderState.Dead)
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
        while (CurrentState != SpiderState.Dead)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            float action = Random.value;
            if (action < 0.33f) // 33% chance to stop
            {
                CurrentState = SpiderState.Idle;
                animator.SetBool("IsMoving", false);
            }
            else if (action < 0.66f) // 33% chance to move left
            {
                CurrentState = SpiderState.Moving;
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
                CurrentState = SpiderState.Moving;
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
    }

    private void UpdateState()
    {
        if (isAttacking) return; // 공격 중일 때는 상태를 변경하지 않음

        float distanceToTarget = Vector3.Distance(Target.position, transform.position);
        if (distanceToTarget <= SpitAttackRange && Time.time >= lastSpitAttackTime + spitAttackCooldown)
        {
            CurrentState = SpiderState.SpitAttack;
            lastSpitAttackTime = Time.time;
        }
        else if (distanceToTarget <= JumpAttackRange && Time.time >= lastJumpAttackTime + jumpAttackCooldown)
        {
            CurrentState = SpiderState.JumpAttack;
            lastJumpAttackTime = Time.time;
        }
        else if (distanceToTarget <= AttackRange)
        {
            CurrentState = SpiderState.Attacking;
        }
        else if (distanceToTarget <= DetectionRange)
        {
            CurrentState = SpiderState.Chasing;
        }
        else
        {
            CurrentState = SpiderState.Idle;
            animator.SetBool("IsMoving", false);
        }
    }

    private void PerformStateAction()
    {
        switch (CurrentState)
        {
            case SpiderState.Chasing:
                ChaseTarget();
                break;
            case SpiderState.Attacking:
                PerformAttack();
                break;
            case SpiderState.JumpAttack:
                PerformJumpAttack();
                break;
            case SpiderState.SpitAttack:
                PerformSpitAttack();
                break;
            case SpiderState.Idle:
                HandleIdle();
                break;
        }
    }

    private void PerformSpitAttack()
    {
        // Debug.Log("Spider performs a spit attack.");
        isAttacking = true;
        FaceDirection((Target.position - transform.position).normalized); // 타겟 방향으로 회전
        animator.SetTrigger("SpitAttack");
        // 침뱉기 공격 로직 구현
        // 애니메이션 이벤트에서 EndAttack 호출 필요
    }

    private void PerformJumpAttack()
    {
        return;
        //Debug.Log("Spider performs a jump attack.");
        //isAttacking = true;
        //animator.SetTrigger("JumpAttack");
        // 점프 공격 로직 구현
        // 애니메이션 이벤트에서 EndAttack 호출 필요
    }

    private void PerformAttack()
    {
        // Debug.Log("Spider is attacking.");
        isAttacking = true;
        FaceDirection((Target.position - transform.position).normalized); // 타겟 방향으로 회전
        animator.SetTrigger("Attack");
        // 기본 공격 로직 구현
        // 애니메이션 이벤트에서 EndAttack 호출 필요
    }

    public void EndAttack()
    {
        // Debug.Log("EndAttack 호출");
        isAttacking = false;
        if (CurrentState != SpiderState.Dead)
        {
            CurrentState = SpiderState.Idle;
        }
    }

    private void ChaseTarget()
    {
        // Debug.Log("Spider is chasing the target.");
        Vector2 direction = (Target.position - transform.position).normalized;
        Move(direction); // 타겟을 쫓아갈 때도 이동 속도 조정
        FaceDirection(direction);
        animator.SetBool("IsMoving", true);
    }

    private void HandleIdle()
    {
        // Debug.Log("Spider is idling.");
        animator.SetBool("IsMoving", false);
    }

    public override void Move(Vector2 direction)
    {
        // 공격 중일 때는 이동하지 않음
        if (isAttacking) return;

        // Debug.Log("Move함수 도중 현재 동작 : " + CurrentState.ToString());
        // Debug.Log("Move함수 도중 변수 값 : isAttacking " + isAttacking.ToString());
        base.Move(direction * 0.05f); // 이동 속도 조정
        animator.SetBool("IsMoving", true);
        FaceDirection(direction);
    }

    private void FaceDirection(Vector2 direction)
    {
        //if (!isAttacking) // 공격 중이 아닐 때만 방향 전환
        //{
            if (direction.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0); // 왼쪽을 바라봄
            }
            else if (direction.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); // 오른쪽을 바라봄
            }
        //}
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (Hp <= 0 && CurrentState != SpiderState.Dead)
        {
            CurrentState = SpiderState.Dead;
            Die();
        }
    }

    public override void Die()
    {
        Console.WriteLine("Enemy died. Type: " + Type);
        // Debug.Log("Spider died.");
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
        // 일회성 몬스터이므로 굳이 오브젝트 풀링이 필요가 없음.
        Destroy(gameObject);
    }

    #region Events


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetTarget(collision.transform);  // 플레이어를 타겟으로 설정
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetTarget(null);  // 플레이어를 타겟으로 설정
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TakeDamage(Target.GetComponent<Player>().AttackPower);  // 몬스터가 피해를 입음
        }
    }

    #endregion
}
