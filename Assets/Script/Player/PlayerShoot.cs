using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform fashe;
    public float bulletSpeed = 10f;
    public ColorType colorType;
    public Color currentColor;// 当前颜色（UI / 子弹使用）

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || fashe == null)
        {
            Debug.Log("子弹组件为空");
            return;
        }

        // 1️鼠标屏幕坐标 → 世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        // 2️计算方向
        Vector2 shootDir = (mouseWorldPos - fashe.position).normalized;

        // 判断角色朝向
        bool faceRight = transform.localScale.x > 0;

        // 3️判断鼠标是否在背后
        if (faceRight && mouseWorldPos.x < transform.position.x) return;
        if (!faceRight && mouseWorldPos.x > transform.position.x) return;

        // 4️生成子弹
        GameObject bullet = Instantiate(bulletPrefab, fashe.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.colorType = colorType;           // ✅ 这里改
            bulletScript.currentColor = GetColor(colorType); // 同步 Sprite 显示颜色

            SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = bulletScript.currentColor;
            }
        }

        if (rb != null)
        {
            rb.velocity = shootDir * bulletSpeed;
        }
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
