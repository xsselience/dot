using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ColorType colorType;
    public float lifeTime = 2f;

    [HideInInspector]
    public Color currentColor;   // 显示用的实际颜色

    void Start()
    {
        // 根据 colorType 获取实际颜色
        currentColor = GetColor(colorType);

        // 设置 SpriteRenderer 颜色
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = currentColor;
        }

        // 子弹在 lifeTime 秒后自动消失
        Destroy(gameObject, lifeTime);
    }

    // 将 ColorType 转换成 Unity Color
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
