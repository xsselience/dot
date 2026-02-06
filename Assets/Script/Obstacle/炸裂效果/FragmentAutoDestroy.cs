using UnityEngine;

public class FragmentAutoDestroy : MonoBehaviour
{
    public float lifeTime = 1.5f;
    public float fadeTime = 0.5f;

    SpriteRenderer sr;
    float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 开始渐隐
        if (timer > lifeTime - fadeTime && sr != null)
        {
            float t = (timer - (lifeTime - fadeTime)) / fadeTime;
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;
        }

        // 时间到，销毁
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
