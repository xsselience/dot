using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GoalTrigger : MonoBehaviour
{
    [Header("下一关场景名")]
    public string nextSceneName;

    [Header("淡出面板")]
    public Image fadePanel;
    public float fadeSpeed = 2f;

    private bool isTriggering = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggering) return;

        if (other.CompareTag("Player"))
        {
            isTriggering = true;
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        Color color = fadePanel.color;

        // 淡出
        while (color.a < 1)
        {
            color.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
