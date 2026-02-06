using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public ColorType colorType;
    public Color currentColor;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("PlayerShoot 必须挂在有 PlayerMovement 的对象上");
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            Shoot();
    }

    void Shoot()
    {
        if (bulletPrefab == null || playerMovement.fashe == null) return;

        // 鼠标世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        // 鼠标相对角色的位置
        float dirX = mouseWorldPos.x - transform.position.x;

        // 角色朝向
        bool faceRight = playerMovement.FaceRight;

        // ⭐ 核心判断：鼠标是否在角色正前方
        if (faceRight && dirX < 0) return; // 面右，但点在左边
        if (!faceRight && dirX > 0) return; // 面左，但点在右边

        // 计算射击方向
        Vector2 shootDir = (mouseWorldPos - playerMovement.fashe.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            playerMovement.fashe.position,
            Quaternion.identity
        );

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.colorType = colorType;
            bulletScript.currentColor = GetColor(colorType);

            SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = bulletScript.currentColor;
        }

        if (rb != null)
            rb.velocity = shootDir * bulletSpeed;

        // 子弹朝向
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    Color GetColor(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red: return Color.red;
            case ColorType.Green: return Color.green;
            case ColorType.Blue: return Color.blue;
            case ColorType.Yellow: return Color.yellow;
            case ColorType.Purple: return new Color(0.5f, 0, 0.5f);
            case ColorType.Cyan: return Color.cyan;
            case ColorType.White: return Color.white;
            case ColorType.Black: return Color.black;
            default: return Color.white;
        }
    }
}
