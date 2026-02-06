using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private WordBlockView view;
    public ColorType colorType = ColorType.Red;  // 障碍物颜色
    private SpriteRenderer sr;
    private Explodable explodable;
    private bool isDestroyed = false; // 防止重复触发

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = GetColor(colorType);

        view = GetComponent<WordBlockView>();
        explodable = GetComponent<Explodable>();
        if (view == null)
            Debug.LogError("WordBlockView missing!");

        UpdateColor();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet == null) return;

        // 黑色子弹 → 不消除
        if (bullet.colorType == ColorType.Black)
        {
            Destroy(bullet.gameObject);
            return;
        }

        // 同色 → 直接炸裂
        if (bullet.colorType == colorType)
        {
            DestroyBlock();
            Destroy(bullet.gameObject);
            return;
        }

        // 异色 → 只做颜色叠加，不炸裂
        colorType = MixColors(colorType, bullet.colorType);
        UpdateColor();
        if (sr != null)
            sr.color = GetColor(colorType);

        Destroy(bullet.gameObject);
    }

    void DestroyBlock()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // 关闭父物体物理与碰撞
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (explodable != null)
        {
            // 确保碎片已经生成
            if (explodable.fragments.Count == 0)
                explodable.fragmentInEditor();

            foreach (GameObject frag in explodable.fragments)
            {
                frag.SetActive(true);
                frag.transform.parent = null;

                frag.transform.localScale = frag.transform.localScale; // 保持原始尺寸

                // 同步颜色
                SpriteRenderer fragSr = frag.GetComponent<SpriteRenderer>();
                if (fragSr != null)
                    fragSr.color = sr != null ? sr.color : GetColor(colorType);

                // 给碎片加力散开
                Rigidbody2D fragRb = frag.GetComponent<Rigidbody2D>();
                if (fragRb != null)
                {
                    Vector2 dir = (frag.transform.position - (Vector3)transform.position).normalized;
                    fragRb.AddForce(dir * 4f, ForceMode2D.Impulse);
                }
            }

            // 隐藏父物体
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 颜色混合规则
    ColorType MixColors(ColorType a, ColorType b)
    {
        if ((a == ColorType.Red && b == ColorType.Green) || (a == ColorType.Green && b == ColorType.Red)) return ColorType.Yellow;
        if ((a == ColorType.Red && b == ColorType.Blue) || (a == ColorType.Blue && b == ColorType.Red)) return ColorType.Purple;
        if ((a == ColorType.Green && b == ColorType.Blue) || (a == ColorType.Blue && b == ColorType.Green)) return ColorType.Cyan;

        if ((a == ColorType.Yellow && b == ColorType.Purple) || (a == ColorType.Purple && b == ColorType.Yellow)) return ColorType.Red;
        if ((a == ColorType.Yellow && b == ColorType.Cyan) || (a == ColorType.Cyan && b == ColorType.Yellow)) return ColorType.Green;
        if ((a == ColorType.Purple && b == ColorType.Cyan) || (a == ColorType.Cyan && b == ColorType.Purple)) return ColorType.Blue;

        if (a == ColorType.White) return b;
        if (b == ColorType.White) return a;

        if (IsBase(a) && IsMix(b)) return IsRelative(a, b) ? ColorType.White : a;
        if (IsBase(b) && IsMix(a)) return IsRelative(b, a) ? ColorType.White : b;

        if (a == ColorType.Black) return ColorType.Black;

        return a;
    }

    void UpdateColor()
    {
        if (view != null)
            view.ApplyColor(GetColor(colorType));
    }

    bool IsBase(ColorType c) => c == ColorType.Red || c == ColorType.Green || c == ColorType.Blue;
    bool IsMix(ColorType c) => c == ColorType.Yellow || c == ColorType.Purple || c == ColorType.Cyan;

    bool IsRelative(ColorType baseColor, ColorType mixColor)
    {
        if (baseColor == ColorType.Red && mixColor == ColorType.Cyan) return true;
        if (baseColor == ColorType.Green && mixColor == ColorType.Purple) return true;
        if (baseColor == ColorType.Blue && mixColor == ColorType.Yellow) return true;
        return false;
    }

    Color GetColor(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red: return Color.red;
            case ColorType.Green: return Color.green;
            case ColorType.Blue: return Color.blue;
            case ColorType.Yellow: return Color.yellow;
            case ColorType.Purple: return new Color(0.5f, 0, 0.5f);
            case ColorType.Cyan: return Color.cyan;
            case ColorType.White: return Color.white;
            case ColorType.Black: return Color.black;
            default: return Color.white;
        }
    }
}
