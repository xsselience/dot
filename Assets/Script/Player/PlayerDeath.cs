using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private bool isDead = false;
    [Header("GameManager 引用")]
    public GameManager gameManager; // 在 Inspector 挂入

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Obstacle") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            // 如果玩家在对方上方
            if (transform.position.y > collision.transform.position.y + 0.3f)
            {
                return;
            }

            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (gameManager != null)
            gameManager.PlayerDied();
        else
            Debug.LogWarning("PlayerDeath: GameManager 未挂载！");
    }
}
