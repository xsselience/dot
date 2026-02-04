using UnityEngine;
using UnityEngine.UI;

public class ColorUI : MonoBehaviour
{
    public PlayerShoot player;

    [Header("颜色 UI（顺序要和 ColorType 对应）")]
    public Image[] colorImages;
    public ColorType[] colorTypes;

    [Header("高亮设置")]
    public Color outlineColor = Color.white;
    public Vector2 outlineDistance = new Vector2(6f, -6f);

    private int currentIndex = 0;

    void Start()
    {
        InitUI();
        UpdateSelection();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            currentIndex = (currentIndex + 1) % colorImages.Length;
            UpdateSelection();
        }
        else if (scroll < 0f)
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = colorImages.Length - 1;

            UpdateSelection();
        }
    }

    void InitUI()
    {
        for (int i = 0; i < colorImages.Length; i++)
        {
            // 设置每个颜色块本身的颜色
            colorImages[i].color = GetColor(colorTypes[i]);

            // 确保有 Outline 组件
            Outline outline = colorImages[i].GetComponent<Outline>();
            if (outline == null)
                outline = colorImages[i].gameObject.AddComponent<Outline>();

            outline.effectColor = outlineColor;
            outline.effectDistance = outlineDistance;
            outline.enabled = false;
        }
    }

    void UpdateSelection()
    {
        ColorType type = colorTypes[currentIndex];

        // 通知玩家当前颜色
        if (player != null)
        {
            player.colorType = type;
        }

        // 更新高亮
        UpdateHighlight();
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < colorImages.Length; i++)
        {
            Outline outline = colorImages[i].GetComponent<Outline>();
            outline.enabled = (i == currentIndex);
        }
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
