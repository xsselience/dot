using UnityEngine;

public class AutoScrollCamera : MonoBehaviour
{
    [Header("目标")]
    public Transform player;
    public PlayerLife playerLife;

    [Header("自动卷轴")]
    public float scrollSpeed = 2f;

    [Header("跟随")]
    public float followOffsetX = 3f; // 玩家超过这个距离，相机开始追
    public float yOffset = 2f;
    public float ySmooth = 3f;

    [Header("GameManager引用")]
    public GameManager gameManager; // 挂在 Inspector

    private float currentY;
    private float cameraX; // 相机“世界进度”

    private bool isBasement = true; // 默认在地下室

    void Start()
    {
        // 查找场景中的 GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogError("PlayerDeath: 场景中找不到 GameManager！");
        currentY = transform.position.y;
        cameraX = transform.position.x;
    }

    void Update()
    {
        MoveCamera();
        CheckKillZone();
    }

    void MoveCamera()
    {
        if (isBasement)
        {
            // 只跟随玩家
            cameraX = player.position.x;
            float targetY = player.position.y + yOffset;
            currentY = Mathf.Lerp(currentY, targetY, ySmooth * Time.deltaTime);
        }
        else
        {
            // 自动卷轴模式
            cameraX += scrollSpeed * Time.deltaTime;
            float followX = player.position.x - followOffsetX;
            cameraX = Mathf.Max(cameraX, followX);

            float targetY = player.position.y + yOffset;
            currentY = Mathf.Lerp(currentY, targetY, ySmooth * Time.deltaTime);
        }

        transform.position = new Vector3(cameraX, currentY, transform.position.z);
    }

    void CheckKillZone()
    {
        float camHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float leftBorder = cameraX - camHalfWidth;

        if (player.position.x < leftBorder)
        {
            if (gameManager != null)
            {
                gameManager.PlayerDied();
            }
        }
    }

    public void ResetCamera(float newX, float newY)
    {
        cameraX = newX;
        currentY = newY;
        transform.position = new Vector3(cameraX, currentY, transform.position.z);
    }

    // Trigger 调用切换地下室状态
    public void SetBasementMode(bool value)
    {
        isBasement = value;
    }
}
