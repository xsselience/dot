using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class Bullet : MonoBehaviour
{
    public ColorType colorType;
    public float lifeTime = 2f;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController redController;
    public RuntimeAnimatorController yellowController;
    public RuntimeAnimatorController blueController;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void ApplyAnimation()
    {
        switch (colorType)
        {
            case ColorType.Red:
                animator.runtimeAnimatorController = redController;
                break;

            case ColorType.Yellow:
                animator.runtimeAnimatorController = yellowController;
                break;

            case ColorType.Blue:
                animator.runtimeAnimatorController = blueController;
                break;
        }
    }
    public void Initialize(ColorType type)
    {
        colorType = type;
        ApplyAnimation();
        Destroy(gameObject, lifeTime);
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
