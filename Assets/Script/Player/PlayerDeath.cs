using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private bool isDead = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Obstacle") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 玩家在上方
                if (contact.normal.y > 0.5f)
                {
                    return; // 从上面踩，不死
                }
            }

            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        GameManager.Instance.PlayerDied();
    }
}
