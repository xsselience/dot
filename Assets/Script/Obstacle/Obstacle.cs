using UnityEngine;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    public ColorType colorType;

    private Explodable explodable;
    private bool isDestroyed = false;

    private HashSet<ColorType> hitColors = new HashSet<ColorType>();


    [Header("破碎音效")] // 新增
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

        // 黑色无效
        if (bulletColor == ColorType.Black)
            return;

        // ===== 基础色障碍物 =====
        if (IsPrimary(colorType))
        {
            if (bulletColor == colorType)
            {
                DestroyBlock();
            }
            return;
        }

        // ===== 混合色障碍物 =====

        if (!IsPrimary(bulletColor))
            return;

        hitColors.Add(bulletColor);

        if (CanDestroyMix())
        {
            DestroyBlock();
        }
    }

    bool IsPrimary(ColorType type)
    {
        return type == ColorType.Red ||
               type == ColorType.Yellow ||
               type == ColorType.Blue;
    }

    bool CanDestroyMix()
    {
        if (hitColors.Count != 2)
            return false;

        switch (colorType)
        {
            case ColorType.Orange:
                return hitColors.SetEquals(
                    new HashSet<ColorType> { ColorType.Red, ColorType.Yellow });

            case ColorType.Green:
                return hitColors.SetEquals(
                    new HashSet<ColorType> { ColorType.Yellow, ColorType.Blue });

            case ColorType.Purple:
                return hitColors.SetEquals(
                    new HashSet<ColorType> { ColorType.Red, ColorType.Blue });

            default:
                return false;
        }
    }

    void DestroyBlock()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // 播放破碎音效
        if (destroyClip != null)
        {
            AudioManager.Instance.PlaySFX(destroyClip);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

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
}
