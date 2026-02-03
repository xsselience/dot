using UnityEngine;
using UnityEngine.UI;

public class ColorUI : MonoBehaviour
{
    public PlayerShoot player;           // 拖 PlayerShoot
    public Image currentColorImage;      // 当前颜色显示框

    public void SelectRed() { SetColor(ColorType.Red); }
    public void SelectBlue() { SetColor(ColorType.Blue); }
    public void SelectGreen() { SetColor(ColorType.Green); }

    private void SetColor(ColorType colorType)
    {
        if (player != null)
        {
            player.colorType = colorType;          
            player.currentColor = GetColor(colorType); // 同步显示颜色
        }

        if (currentColorImage != null)
        {
            currentColorImage.color = GetColor(colorType); // 更新 UI 显示
        }
    }

    private Color GetColor(ColorType type)
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
