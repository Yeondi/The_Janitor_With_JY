using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    public Weapon EquippedWeapon { get; set; }
    public Item EquippedItem { get; set; }

    [SerializeField]
    private Vector3 moveDirection;
    [SerializeField]
    private float jumpPower = 7f;
    [Tooltip("기어가는 속도")]
    [SerializeField]
    private float crawlSpeed = 0f;
    [Tooltip("떨어지는 속도")]
    [SerializeField]
    private readonly float fallSpeed = 1.5f;
    [Tooltip("현재 플레이어의 속도 / 각 동작별 속도를 여기에 대입")]
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private bool isOnGrounded = true;
    [SerializeField]
    private bool isSprinting = false;
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private bool isOverlappedByLadder = false;
    private bool canPick = false;
    private IInteractable currentInteractable;

    private Animator animator;
    private Transform playerTransform;
    private Rigidbody2D rb;
    public InputAction playerActions;

    private float lastActionTime = 0f;
    private float idleDuration = 10f; // Idle 상태 유지 시간

    public TMP_Text test;

    public InventoryManager inventoryManager;

    private void OnEnable()
    {
        EventManager.StartListening("ClueCollected", OnClueCollected);
    }

    private void OnDisable()
    {
        EventManager.StopListening("ClueCollected", OnClueCollected);
    }

    private void Start()
    {
        playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        crawlSpeed = MovementSpeed * 0.7f;
        lastActionTime = Time.time;
    }

    private void Update()
    {
        // 10초 동안 아무런 행동이 없으면 기침 애니메이션 트리거
        if (Time.time - lastActionTime >= idleDuration)
        {
            animator.SetTrigger("Cough");
            lastActionTime = Time.time; // 다시 초기화
        }
    }

    public override void Attack(Character target)
    {
        if (EquippedWeapon != null)
        {
            EquippedWeapon.Use(this);
        }
        else
        {
            Debug.Log("Attack " + name);
            Vector3 myPos = playerTransform.position;
            EffectsManager.Instance.PlayEffect(name, new Vector3(myPos.x + playerTransform.localScale.x, myPos.y + 0.5f, 1));
        }
    }

    public void UseItem()
    {
        if (EquippedItem != null)
        {
            EquippedItem.Use(this);
        }
    }

    private void FixedUpdate()
    {
        float gravityMultiplier = isJumping && rb.velocity.y < 0 ? 1.2f : 1.0f;
        rb.velocity += Vector2.up * Physics2D.gravity.y * gravityMultiplier * Time.fixedDeltaTime;

        currentSpeed = isSprinting ? SprintSpeed : MovementSpeed;
        rb.velocity = new Vector2(moveDirection.x * currentSpeed, rb.velocity.y);
    }

    public void PlayEffect(string fxName)
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveDirection = new Vector2(input.x, 0);
        float moveValue = Mathf.Abs(input.x);
        animator.SetFloat("MoveValue", moveValue);

        bool isMoving = moveValue > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        lastActionTime = Time.time;

        if (input.x != 0)
        {
            playerTransform.localScale = new Vector3(4 * Mathf.Sign(input.x), 4 * 1, 1);

            if (isSprinting)
            {
                currentSpeed = SprintSpeed;
                animator.SetBool("SprintStart", true);
            }
            else
            {
                currentSpeed = MovementSpeed;
                animator.SetBool("SprintStart", false);
            }
        }
        else
        {
            currentSpeed = 0;
            animator.SetFloat("MoveValue", 0f);
            animator.SetBool("IsMoving", false);
            animator.SetBool("SprintStart", false);
        }

        if (input.y < 0)
        {
            animator.SetBool("IsCrawl", true);
            currentSpeed = crawlSpeed;
        }
        else if (input.y > 0)
        {
            if (isOverlappedByLadder)
                animator.SetBool("IsClimbing", true);
        }
        else
        {
            animator.SetBool("IsCrawl", false);
            animator.SetBool("IsClimbing", false);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && isOnGrounded)
        {
            lastActionTime = Time.time;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isOnGrounded = false;
            isJumping = true;
            animator.SetTrigger("OnJump");
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            lastActionTime = Time.time;
            isSprinting = true;
            animator.SetBool("SprintStart", true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSprinting = false;
            animator.SetBool("SprintStart", false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnClueCollected(object clueInfo)
    {
        //int collectedClueId = (int)clueInfo;
        int collectedClueId = int.Parse(clueInfo.ToString());
        Debug.Log("단서 획득: " + clueInfo);

        if (ClueManager.Instance != null)
        {
            /*
            1. 클루 데이터 뭉치기
            2. 뜯어서 아이디 다른 애들만 생성
            */
            //ClueManager.Instance.CollectClue(collectedClueId);

            inventoryManager.AddClue(collectedClueId);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGrounded = true;
            if (isJumping)
            {
                animator.SetBool("IsOnGround", true);
                isJumping = false;
            }
        }
        else if (collision.gameObject.CompareTag("Clue"))
        {
            canPick = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGrounded = false;
            if (isJumping)
            {
                animator.SetBool("IsOnGround", false);
                isJumping = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() != null)
        {
            currentInteractable = collision.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() != null)
        {
            if (currentInteractable == collision.GetComponent<IInteractable>())
            {
                currentInteractable = null;
            }
        }
    }
}
