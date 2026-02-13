using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    // 玩家死亡事件（依然可以被其他脚本订阅）
    public static event Action OnPlayerDied;

    [Header("死亡UI")]
    public GameObject deathUI;

    private void Awake()
    {
        // 确保 deathUI 引用
        if (deathUI == null)
            deathUI = GameObject.Find("DeathUI");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 每次场景加载都重新找 deathUI
        if (deathUI == null)
        {
            deathUI = GameObject.Find("DeathUI");
        }

        Time.timeScale = 1f;
    }

    // 玩家死亡
    public void PlayerDied()
    {
        Debug.Log("GameManager PlayerDied called");

        Time.timeScale = 0f;

        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }

        OnPlayerDied?.Invoke();
    }

    // 重启本关
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 返回主菜单
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
