using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed = 5f;

    [Header("跳跃")]
    public float jumpHigh = 25f;
    public float secondJumpTimeLimit = 0.5f;

    [Header("Jump Feel")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("落地检测")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
    public LayerMask groundLayer;

    [Header("发射点")]
    public Transform fashe;

    public bool IsGrounded => isGrounded;
    public bool FaceRight => sr != null ? !sr.flipX : true;
    public float MoveInput { get; private set; }

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool isGrounded;
    private int jumpCount;
    private float secondJumpTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
            Debug.LogError("Player 子物体中找不到 SpriteRenderer");

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.sharedMaterial = new PhysicsMaterial2D
        {
            friction = 0f,
            bounciness = 0f
        };
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // 二段跳时间窗口
        if (secondJumpTimer > 0)
            secondJumpTimer -= Time.deltaTime;

        // 跳跃输入
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryJump();
        }
    }

    void FixedUpdate()
    {
        if (Keyboard.current == null) return;

        CheckGrounded();
        HandleMove();
        HandleJumpFeel();
    }

    // =========================
    // 地面检测（稳定核心）
    // =========================
    void CheckGrounded()
    {
        bool groundedNow = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        // 只有从空中落地才重置跳跃次数
        if (groundedNow && !isGrounded)
        {
            jumpCount = 0;
            secondJumpTimer = 0f; // 二段跳计时器归零
        }

        isGrounded = groundedNow;
    }


    // =========================
    // 移动 & 翻转
    // =========================
    void HandleMove()
    {
        float move = 0f;
        if (Keyboard.current.aKey.isPressed) move = -1f;
        else if (Keyboard.current.dKey.isPressed) move = 1f;

        MoveInput = move;
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if (move > 0.01f)
        {
            sr.flipX = false;
            FlipFashe(true);
        }
        else if (move < -0.01f)
        {
            sr.flipX = true;
            FlipFashe(false);
        }
    }

    // =========================
    // 跳跃逻辑（地面 + 二段）
    // =========================
    void TryJump()
    {
        if (isGrounded)
        {
            Jump();
            jumpCount = 1;               // 第一次跳
            secondJumpTimer = secondJumpTimeLimit; // 二段跳时间窗口
        }
        else if (jumpCount == 1 && secondJumpTimer > 0f)
        {
            Jump();
            jumpCount = 2;               // 二段跳完成
            secondJumpTimer = 0f;        // 禁止再次跳
        }
        // 其他情况不能跳
    }


    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpHigh);
    }

    // =========================
    // 跳跃手感优化
    // =========================
    void HandleJumpFeel()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y
                * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y
                * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    // =========================
    // 发射点翻转
    // =========================
    void FlipFashe(bool facingRight)
    {
        if (fashe == null) return;

        Vector3 pos = fashe.localPosition;
        pos.x = facingRight ? Mathf.Abs(pos.x) : -Mathf.Abs(pos.x);
        fashe.localPosition = pos;
    }

    // =========================
    // 可视化地面检测
    // =========================
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
