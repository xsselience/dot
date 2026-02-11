using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("UI")]
    public Button level2Button;

    public Image orangeIcon;
    public Image greenIcon;
    public Image purpleIcon;

    [Header("颜色")]
    public Color collectedColor = Color.white;
    public Color unCollectedColor = Color.gray;


    [Header("团队UI")]
    public GameObject teamUIPanel;

    [Header("展示品UI")]
    public GameObject collectionUIPanel;

    void Start()
    {
        // 确保团队面板默认关闭
        if (teamUIPanel != null)
            teamUIPanel.SetActive(false);
        if (collectionUIPanel != null)
            collectionUIPanel.SetActive(false);
        RefreshUI();
    }

    void RefreshUI()
    {
        // 刷新颜色图标
        orangeIcon.color = ColorCollectManager.HasOrange ? collectedColor : unCollectedColor;
        greenIcon.color = ColorCollectManager.HasGreen ? collectedColor : unCollectedColor;
        purpleIcon.color = ColorCollectManager.HasPurple ? collectedColor : unCollectedColor;

        // 刷新第二关按钮
        bool canEnterLevel2 = ColorCollectManager.AllCollected();
        level2Button.interactable = canEnterLevel2;

        // 顺便把按钮变灰（可选但推荐）
        level2Button.image.color = canEnterLevel2 ? Color.white : Color.gray;
    }

    public void StartLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void StartLevel2()
    {
        if (!ColorCollectManager.AllCollected())
            return;

        SceneManager.LoadScene("Level2");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void ShowTeamUI()
    {
        teamUIPanel.SetActive(true);
    }

    public void HideTeamUI()
    {
        teamUIPanel.SetActive(false);
    }

    public void ShowCollectionUI()
    {
        collectionUIPanel.SetActive(true);
    }

    public void HideCollectionUI()
    {
        collectionUIPanel.SetActive(false);
    }
}
