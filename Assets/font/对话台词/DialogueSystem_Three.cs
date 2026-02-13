using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueSystem_Three : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    public Image characterA;
    public Image characterB;
    public Image characterC;

    public string[] sentences;

    // 0 = A
    // 1 = B
    // 2 = C
    // 3 = 无人
    public int[] speaker;

    [Header("UI 切换")]
    public GameObject dialogueUI;   // 整个对话界面
    public GameObject background;
    public GameObject returnMenu;

    private int index = 0;
    private bool finished = false;

    void Start()
    {
        ShowSentence();
        returnMenu.SetActive(false);
    }

    void Update()
    {
        if (finished) return;

        if (Input.GetMouseButtonDown(0))
        {
            index++;

            if (index < sentences.Length)
            {
                ShowSentence();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    void ShowSentence()
    {
        dialogueText.text = sentences[index];

        // 全部变暗
        Color dark = new Color(0.6f, 0.6f, 0.6f, 1f);
        characterA.color = dark;
        characterB.color = dark;
        characterC.color = dark;

        // 高亮说话者
        if (speaker[index] == 0)
            characterA.color = Color.white;
        else if (speaker[index] == 1)
            characterB.color = Color.white;
        else if (speaker[index] == 2)
            characterC.color = Color.white;
    }

    void EndDialogue()
    {
        finished = true;

        // 隐藏对话UI
        dialogueUI.SetActive(false);
        returnMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        Debug.Log("按钮被点击了");
        SceneManager.LoadScene("MainMenu");
    }
}
