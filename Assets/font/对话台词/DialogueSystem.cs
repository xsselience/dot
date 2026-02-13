using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    public Image leftCharacter;
    public Image rightCharacter;

    public string[] sentences;

    public enum SpeakerType
    {
        Left,
        Right,
        Other
    }

    public SpeakerType[] speakerType;

    [Header("场景跳转")]
    public string nextSceneName;
    public Image fadePanel;
    public float fadeSpeed = 2f;

    private int index = 0;

    void Start()
    {
        ShowSentence();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            index++;

            if (index < sentences.Length)
            {
                ShowSentence();
            }
            else
            {
                StartCoroutine(FadeAndLoadScene());
            }
        }
    }

    void ShowSentence()
    {
        dialogueText.text = sentences[index];

        leftCharacter.color = Color.gray;
        rightCharacter.color = Color.gray;

        if (speakerType[index] == SpeakerType.Left)
        {
            leftCharacter.color = Color.white;
        }
        else if (speakerType[index] == SpeakerType.Right)
        {
            rightCharacter.color = Color.white;
        }
    }

    IEnumerator FadeAndLoadScene()
    {
        Color color = fadePanel.color;

        // 淡出（变黑）
        while (color.a < 1)
        {
            color.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
