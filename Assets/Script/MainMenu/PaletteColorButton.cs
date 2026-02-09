using UnityEngine;
using UnityEngine.UI;

public class PaletteColorButton : MonoBehaviour
{
    public ColorType colorType;
    public Image image;
    public Button button;

    public Color lockedColor = Color.gray;

    void Awake()
    {
        // ∑¿÷πÕ¸º«Õœ“˝”√
        if (image == null)
            image = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();
    }

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (ColorUnlockManager.Instance == null)
            return;

        bool unlocked = ColorUnlockManager.Instance.IsUnlocked(colorType);

        image.color = unlocked ? GetColorByType(colorType) : lockedColor;
        button.interactable = unlocked;

        Debug.Log($"{name} unlocked = {unlocked}, showColor = {image.color}");
    }

    public void OnClick()
    {
        PaintManager.Instance.SetCurrentColor(GetColorByType(colorType));
    }

    Color GetColorByType(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red:
                return Color.red;
            case ColorType.Green:
                return Color.green;
            case ColorType.Blue:
                return Color.blue;
            case ColorType.Yellow:
                return Color.yellow;
            case ColorType.Cyan:
                return Color.cyan;
            case ColorType.Purple:
                return new Color(0.5f, 0f, 0.5f);
            case ColorType.White:
                return Color.white;
            case ColorType.Black:
                return Color.black;
            default:
                return Color.white;
        }
    }
}
