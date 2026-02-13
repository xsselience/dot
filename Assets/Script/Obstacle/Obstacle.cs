using UnityEngine;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    public ColorType colorType;

    private Explodable explodable;
    private bool isDestroyed = false;

    private HashSet<ColorType> hitColors = new HashSet<ColorType>();

    [Header("破碎音效")]
    public AudioClip destroyClip;

    void Start()
    {
        explodable = GetComponent<Explodable>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed) return;

        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet == null) return;

        ColorType bulletColor = bullet.colorType;
        Destroy(collision.gameObject);

        if (bulletColor == ColorType.Black)
            return;

        // ===== 基础色障碍物 =====
        if (IsPrimary(colorType))
        {
            if (bulletColor == colorType)
                DestroyBlock();
            return;
        }

        // ===== 混合色障碍物 =====
        ColorType[] requiredColors = GetRequiredColors(colorType);
        if (requiredColors == null) return;

        // 只记录必要的颜色
        if (System.Array.Exists(requiredColors, c => c == bulletColor))
        {
            hitColors.Add(bulletColor);
        }

        if (CanDestroyMix())
            DestroyBlock();
    }

    // 判断是否为基础色
    bool IsPrimary(ColorType type)
    {
        return type == ColorType.Red ||
               type == ColorType.Yellow ||
               type == ColorType.Blue;
    }

    // 获取混合色障碍物所需原色
    ColorType[] GetRequiredColors(ColorType mix)
    {
        switch (mix)
        {
            case ColorType.Orange: return new ColorType[] { ColorType.Red, ColorType.Yellow };
            case ColorType.Green: return new ColorType[] { ColorType.Yellow, ColorType.Blue };
            case ColorType.Purple: return new ColorType[] { ColorType.Red, ColorType.Blue };
            default: return null;
        }
    }

    // 判断混合色障碍物是否满足破坏条件
    bool CanDestroyMix()
    {
        ColorType[] requiredColors = GetRequiredColors(colorType);
        if (requiredColors == null) return false;

        return hitColors.SetEquals(new HashSet<ColorType>(requiredColors));
    }

    // 障碍物破坏逻辑
    void DestroyBlock()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // 播放破碎音效
        if (destroyClip != null)
        {
            AudioManager.Instance.PlaySFX(destroyClip);
        }

        // 关闭刚体和碰撞
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // 爆裂碎片
        if (explodable != null)
        {
            if (explodable.fragments.Count == 0)
                explodable.fragmentInEditor();

            foreach (GameObject frag in explodable.fragments)
            {
                frag.SetActive(true);
                frag.transform.parent = null;

                Rigidbody2D fragRb = frag.GetComponent<Rigidbody2D>();
                if (fragRb != null)
                {
                    Vector2 dir = (frag.transform.position - transform.position).normalized;
                    fragRb.AddForce(dir * 4f, ForceMode2D.Impulse);
                }
            }

            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 可选：重置障碍物时清空记录
    public void ResetObstacle()
    {
        isDestroyed = false;
        hitColors.Clear();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        gameObject.SetActive(true);
    }
}
