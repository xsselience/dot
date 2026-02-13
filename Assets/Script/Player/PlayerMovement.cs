using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; 


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

    [Header("音效")]
    public AudioClip footstepClip;
    public AudioClip firstJumpClip;
    public AudioClip secondJumpClip;
    private AudioSource footstepSource;

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
        // 初始化脚步音 AudioSource
        if (footstepClip != null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;          // 循环播放
            footstepSource.playOnAwake = false;  // 不自动播放
            footstepSource.volume = 2f;        // 可调节
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;   // 新增
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

    // 地面检测
    void CheckGrounded()
    {
        // 检测 Box 范围内所有碰撞体
        Collider2D[] hits = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0f);

        bool groundedNow = false;

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            // 普通地面 Layer
            if (((1 << hit.gameObject.layer) & groundLayer) != 0)
            {
                groundedNow = true;
                break;
            }

            // 颜色平台
            ColorPlatform cp = hit.GetComponent<ColorPlatform>();
            if (cp != null && hit.enabled)
            {
                // 玩家脚底 Y 坐标
                float playerFeet = groundCheck.position.y;
                // 平台顶部 Y 坐标
                float platformTop = hit.bounds.max.y;

                // 玩家脚底在平台上或接触平台，算踩上去
                if (playerFeet >= platformTop - 0.05f)
                {
                    groundedNow = true;
                    break;
                }
            }

            // 其他可踩障碍物
            if (hit.CompareTag("Obstacle"))
            {
                float playerFeet = groundCheck.position.y;
                float top = hit.bounds.max.y;

                if (playerFeet >= top - 0.05f)
                {
                    groundedNow = true;
                    break;
                }
            }
        }

        // 只有从空中落地才重置跳跃次数
        if (groundedNow && !isGrounded)
        {
            jumpCount = 0;
            secondJumpTimer = 0f;
        }

        isGrounded = groundedNow;
    }

    void HandleMove()
    {
        float move = 0f;
        if (Keyboard.current.aKey.isPressed) move = -1f;
        else if (Keyboard.current.dKey.isPressed) move = 1f;

        MoveInput = move;
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // 翻转
        if (sr != null)
        {
            if (move > 0.01f) { sr.flipX = false; FlipFashe(true); }
            else if (move < -0.01f) { sr.flipX = true; FlipFashe(false); }
        }

        // 播放/停止脚步音
        if (footstepSource != null)
        {
            if (move != 0 && isGrounded)
            {
                if (!footstepSource.isPlaying)
                    footstepSource.Play();
            }
            else
            {
                if (footstepSource.isPlaying)
                    footstepSource.Stop();
            }
        }
    }


    // 跳跃逻辑（地面 + 二段）
    void TryJump()
    {
        if (isGrounded)
        {
            Jump();
            jumpCount = 1;               // 第一次跳
            secondJumpTimer = secondJumpTimeLimit; // 二段跳时间窗口
            if (firstJumpClip != null)
                AudioManager.Instance.PlaySFX(firstJumpClip);
        }
        else if (jumpCount == 1 && secondJumpTimer > 0f)
        {
            Jump();
            jumpCount = 2;               // 二段跳完成
            secondJumpTimer = 0f;        // 禁止再次跳
            if (secondJumpClip != null)
                AudioManager.Instance.PlaySFX(secondJumpClip);
        }
        // 其他情况不能跳
    }


    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpHigh);
    }

    // 跳跃手感优化
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

    // 发射点翻转
    void FlipFashe(bool facingRight)
    {
        if (fashe == null) return;

        Vector3 pos = fashe.localPosition;
        pos.x = facingRight ? Mathf.Abs(pos.x) : -Mathf.Abs(pos.x);
        fashe.localPosition = pos;
    }


    // 可视化地面检测
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
