using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [Header("∏¥ªÓ…Ë÷√")]
    public Transform respawnPoint;
    public AutoScrollCamera cameraController;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Die()
    {
        Debug.Log("Player Dead");
        Respawn();
    }

    void Respawn()
    {
        rb.velocity = Vector2.zero;
        transform.position = respawnPoint.position;

        cameraController.ResetCamera(respawnPoint.position.x);
    }
}
