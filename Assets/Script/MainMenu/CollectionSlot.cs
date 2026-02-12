using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionSlot : MonoBehaviour
{
    public string collectibleID;
    public Image itemImage;
    [TextArea] public string description;

    public TMP_Text descriptionTextUI;
    public GameObject descriptionPanel;

    [HideInInspector]
    public bool isUnlocked;   //由 CollectionPage 控制

    public void SetDisplay(bool unlocked)
    {
        isUnlocked = unlocked;   //记录状态
        itemImage.enabled = unlocked;
    }

    public void OnClick()
    {
        Debug.Log("按钮被点击了！");
        Debug.Log("isUnlocked: " + isUnlocked);

        if (!isUnlocked)
            return;

        if (descriptionPanel != null && descriptionTextUI != null)
        {
            descriptionPanel.SetActive(true);
            descriptionTextUI.text = description;
        }
    }

    public void CloseDescription()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }
}
