using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHigh = 8f;
    public float secondJumptime = 0.5f;

    private Rigidbody2D body;
    private bool isGrounded;
    private int jumpCount;
    private float secondJumpTime;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (secondJumpTime > 0)
            secondJumpTime -= Time.deltaTime;

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
        if (Keyboard.current == null)
            return;

        float move = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            move -= 1f;
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            move += 1f;
            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }

        body.velocity = new Vector2(move * moveSpeed, body.velocity.y);
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
