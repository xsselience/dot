using UnityEngine;

public class AutoScrollCamera : MonoBehaviour
{
    public Transform player;
    public float scrollSpeed = 2f;
    public float yOffset = 2f;
    public float ySmooth = 3f;

    private float currentY;

    void Start()
    {
        currentY = transform.position.y;
    }

    void Update()
    {
        transform.position += Vector3.right * scrollSpeed * Time.deltaTime;

        float targetY = Mathf.Clamp(player.position.y + yOffset, 0f, 10f);

        currentY = Mathf.Lerp(currentY, targetY, ySmooth * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
    }
}
