using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ColorType colorType;
    public float lifeTime = 2f;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        ApplyColor();
        Destroy(gameObject, lifeTime);
    }

    void ApplyColor()
    {
        if (sr == null) return;

        sr.color = GetColorByType(colorType);
    }

    Color GetColorByType(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red:
                return Color.red;

            case ColorType.Yellow:
                return Color.yellow;

            case ColorType.Blue:
                return Color.blue;

            default:
                return Color.white;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
