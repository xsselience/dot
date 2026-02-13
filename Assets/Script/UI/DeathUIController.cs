using UnityEngine;

public class DeathUIController : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameObject.SetActive(false);

        // 自动查找场景里的 GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogError("DeathUIController: 场景里没有 GameManager！");
    }

    void OnEnable()
    {
        if (gameManager != null)
            GameManager.OnPlayerDied += Show;
    }

    void OnDisable()
    {
        if (gameManager != null)
            GameManager.OnPlayerDied -= Show;
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    // 按钮用
    public void Restart()
    {
        if (gameManager != null)
            gameManager.RestartGame();
    }

    public void BackToMenu()
    {
        if (gameManager != null)
            gameManager.BackToMainMenu();
    }
}
