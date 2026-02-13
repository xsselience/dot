using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
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
    }
    public void StartLevel1()
    {
        SceneManager.LoadScene("undergrounddialogue");
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
