using UnityEngine;

public class ColorPickup : MonoBehaviour
{
    public ColorType colorType; // 预设颜色
    public SpriteRenderer fillRenderer;

    void Start()
    {
        fillRenderer.color = GetColor(colorType);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerAmmo playerAmmo = other.GetComponent<PlayerAmmo>();
        if (playerAmmo == null) return;

        //只有当前选中颜色匹配时才能拾取
        if (playerAmmo.currentColor != colorType)
        {
            Debug.Log("当前未选中该颜色，无法拾取");
            return;
        }

        // 增加子弹数量
        playerAmmo.AddAmmo(colorType, 3);

        Destroy(gameObject);
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
