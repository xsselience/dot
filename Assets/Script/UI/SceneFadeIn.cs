using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadePanel;
    public float fadeSpeed = 2f;

    IEnumerator Start()
    {
        Color color = fadePanel.color;
        color.a = 1;
        fadePanel.color = color;

        while (color.a > 0)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            fadePanel.color = color;
            yield return null;
        }
    }
}
