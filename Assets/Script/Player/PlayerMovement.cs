using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动 & 跳跃参数")]
    public float moveSpeed = 5f;
    public float jumpHigh = 20f;
    public float secondJumptime = 0.5f;

    [Header("发射点")]
    public Transform fashe; // 发射点

    public bool IsGrounded => isGrounded;
    public bool FaceRight => sr != null ? !sr.flipX : true;
    public float MoveInput { get; private set; }

    private Rigidbody2D body;
    private SpriteRenderer sr;

    private bool isGrounded;
    private int jumpCount = 0;
    private float secondJumpTime = 0f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
            Debug.LogError("在 Player 的子物体中找不到 SpriteRenderer");

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        PhysicsMaterial2D mat = new PhysicsMaterial2D { friction = 0f, bounciness = 0f };
        col.sharedMaterial = mat;

        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (secondJumpTime > 0) secondJumpTime -= Time.deltaTime;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isGrounded)
            {
                body.velocity = new Vector2(body.velocity.x, jumpHigh);
                jumpCount = 1;
                secondJumpTime = secondJumptime;
            }
            else if (jumpCount == 1 && secondJumpTime > 0)
            {
                body.velocity = new Vector2(body.velocity.x, jumpHigh);
                jumpCount = 2;
                secondJumpTime = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (Keyboard.current == null) return;

        float move = 0f;
        if (Keyboard.current.aKey.isPressed) move = -1f;
        else if (Keyboard.current.dKey.isPressed) move = 1f;

        MoveInput = move;
        body.velocity = new Vector2(move * moveSpeed, body.velocity.y);

        // 翻转角色
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

    // 发射点左右翻转
    private void FlipFashe(bool facingRight)
    {
        if (fashe == null) return;
        Vector3 localPos = fashe.localPosition;
        localPos.x = facingRight ? Mathf.Abs(localPos.x) : -Mathf.Abs(localPos.x);
        fashe.localPosition = localPos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
