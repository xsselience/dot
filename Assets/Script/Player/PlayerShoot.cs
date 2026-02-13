using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("射击设置")]
    public GameObject bulletPrefab;   // 子弹 Prefab
    public float bulletSpeed = 10f;   // 子弹速度

    private PlayerMovement playerMovement;
    private PlayerAmmo playerAmmo;

    [Header("音效")]
    public AudioClip shootClip; // 射击音效
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAmmo = GetComponent<PlayerAmmo>();

        if (playerMovement == null)
            Debug.LogError("PlayerShoot 必须挂在有 PlayerMovement 的对象上");

        if (playerAmmo == null)
            Debug.LogError("PlayerShoot 必须挂在有 PlayerAmmo 的对象上");
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || playerMovement.fashe == null) return;

        // ① 从 PlayerAmmo 读取当前颜色
        ColorType color = playerAmmo.currentColor;

        // ② 子弹不足就不射
        if (!playerAmmo.UseAmmo(color, 1))
            return;

        // ③ 鼠标世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        mouseWorldPos.z = 0f;

        // ④ 射击方向
        Vector2 shootDir = (mouseWorldPos - playerMovement.fashe.position).normalized;

        // ⑤ 生成子弹
        GameObject bullet = Instantiate(
            bulletPrefab,
            playerMovement.fashe.position,
            Quaternion.identity
        );

        // ⑥ 设置速度
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = shootDir * bulletSpeed;

        // ⑦ 设置子弹颜色类型
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(color);
        }

        // 播放射击音效
        if (shootClip != null)
            AudioManager.Instance.PlaySFX(shootClip);

        // ⑧ 子弹朝向
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
