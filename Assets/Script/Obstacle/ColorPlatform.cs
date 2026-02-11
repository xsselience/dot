using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ColorPlatform : MonoBehaviour
{
    public ColorType platformColor;  // 平台颜色
    public int ammoGain = 1;

    private Collider2D col;
    private SpriteRenderer sr;
    private bool activated = false;  // 防止重复发子弹

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        col.isTrigger = false; // 默认可以站上去
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (activated) return;

        PlayerAmmo playerAmmo = collision.gameObject.GetComponent<PlayerAmmo>();
        if (playerAmmo == null) return;

        // 玩家当前颜色匹配
        if (playerAmmo.currentColor == platformColor)
        {
            // 发子弹
            playerAmmo.AddAmmo(platformColor, ammoGain);
            activated = true;

            // 让平台消失
            StartCoroutine(DisappearAndDrop());
        }
    }

    System.Collections.IEnumerator DisappearAndDrop()
    {
        // 先禁用Collider，玩家掉下去
        col.enabled = false;

        // 可选：淡出平台效果
        float duration = 0.3f;
        float elapsed = 0f;
        Color original = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            sr.color = new Color(original.r, original.g, original.b, Mathf.Lerp(1f, 0f, elapsed / duration));
            yield return null;
        }

        // 确保消失
        sr.enabled = false;
        Destroy(gameObject, 0.1f); // 删除物体
    }
}
