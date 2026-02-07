using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action OnPlayerDied;

    [Header("����UI")]
    public GameObject deathUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ��������ʱ�ص����������°�UI
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ÿ�γ������ض������
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (deathUI == null)
        {
            deathUI = GameObject.Find("DeathUI"); // ����������UI������
        }
        // ֻ�ָ���Ϸ�ٶ�
        Time.timeScale = 1f;
        // ���޸� deathUI
    }

    // �������
    public void PlayerDied()
    {
        Debug.Log("��GM��PlayerDied called");

        // ��ͣ��Ϸ
        Time.timeScale = 0f;

        // ��ʾUI
        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }

        // �����¼������������ϵͳ������
        OnPlayerDied?.Invoke();
    }

    // ���¿�ʼ��ǰ����
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // �������˵�
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // ע���滻�������˵�������
    }
}
