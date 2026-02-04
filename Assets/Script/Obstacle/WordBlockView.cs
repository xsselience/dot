using UnityEngine;
using TMPro;

public class WordBlockView : MonoBehaviour
{
    public Renderer backgroundRenderer;
    public TextMeshPro textMesh;

    public void ApplyColor(Color bgColor)
    {
        // 设置背景颜色
        if (backgroundRenderer != null)
            backgroundRenderer.material.color = bgColor;

        // 设置文字颜色（自动对比）
        if (textMesh != null)
            textMesh.color = GetContrastColor(bgColor);
    }

    Color GetContrastColor(Color c)
    {
        // 计算亮度
        float brightness = (c.r * 0.299f + c.g * 0.587f + c.b * 0.114f);

        // 暗色背景 → 白字
        if (brightness < 0.5f)
            return Color.white;
        else
            return Color.black;
    }
}
