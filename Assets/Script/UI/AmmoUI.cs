using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("引用")]
    public PlayerAmmo playerAmmo;

    [Header("顶部刷子图片（不用改颜色）")]
    public RectTransform[] brushRects;

    [Header("UI 组件")]
    public Image[] colorBoxes;      // 红 黄 蓝
    public TMP_Text[] countTexts;   // 红 黄 蓝

    [Header("颜色顺序")]
    public ColorType[] colors =
    {
        ColorType.Red,
        ColorType.Yellow,
        ColorType.Blue
    };

    private int currentIndex = 0;

    void Start()
    {
        if (playerAmmo == null)
        {
            Debug.LogError("AmmoUI 没有绑定 PlayerAmmo");
            enabled = false;
            return;
        }

        SyncIndexWithPlayer();
        UpdateUI();
    }

    void Update()
    {
        HandleScroll();
        UpdateUI();
    }

    void HandleScroll()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0)
            currentIndex = (currentIndex + 1) % colors.Length;
        else if (scroll < 0)
            currentIndex = (currentIndex - 1 + colors.Length) % colors.Length;

        playerAmmo.currentColor = colors[currentIndex];
    }

    void UpdateUI()
    {
        for (int i = 0; i < colors.Length; i++)
        {
        ColorType type = colors[i];

        // 更新数量
        countTexts[i].text = playerAmmo.GetAmmo(type).ToString();

        // 先获取对应颜色
        Color baseColor = GetColor(type);

        bool isSelected = (type == playerAmmo.currentColor);

        if (isSelected)
            colorBoxes[i].color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        else
            colorBoxes[i].color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.3f);
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

    void SyncIndexWithPlayer()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i] == playerAmmo.currentColor)
            {
                currentIndex = i;
                break;
            }
        }
    }
}
