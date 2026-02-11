using UnityEngine;

public class ColorPickup : MonoBehaviour
{
    public ColorType colorType; // 预设颜色
    public SpriteRenderer fillRenderer;
    private Vector3 startPos;

    void Start()
    {
        fillRenderer.color = GetColor(colorType);
        startPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 玩家拾取
        PlayerAmmo playerAmmo = other.GetComponent<PlayerAmmo>();
        if (playerAmmo != null)
        {
            // 增加子弹数量
            playerAmmo.AddAmmo(colorType, 3);
            //切换颜色
            playerAmmo.currentColor = colorType;

            Destroy(gameObject);
        }
    }

    Color GetColor(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red: return Color.red;
            case ColorType.Yellow: return Color.yellow;
            case ColorType.Blue: return Color.blue;
            default: return Color.white;
        }
    }
}
