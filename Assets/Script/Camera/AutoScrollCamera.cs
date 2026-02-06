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

    private float currentY;
    private float cameraX; // 相机“世界进度”

    void Start()
    {
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
        // 1️自动卷轴推进
        cameraX += scrollSpeed * Time.deltaTime;

        // 2️如果玩家跑得太快，相机追上去
        float followX = player.position.x - followOffsetX;
        cameraX = Mathf.Max(cameraX, followX);

        // 3️Y 轴平滑
        float targetY = Mathf.Clamp(player.position.y + yOffset, 0f, 10f);
        currentY = Mathf.Lerp(currentY, targetY, ySmooth * Time.deltaTime);

        transform.position = new Vector3(cameraX, currentY, transform.position.z);
    }

    void CheckKillZone()
    {
        float camHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float leftBorder = cameraX - camHalfWidth;

        if (player.position.x < leftBorder)
        {
            playerLife.Die();
        }
    }

    // 给 PlayerLife 调用
    public void ResetCamera(float newX)
    {
        cameraX = newX;
        transform.position = new Vector3(cameraX, currentY, transform.position.z);
    }
}
