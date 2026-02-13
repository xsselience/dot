using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalEggSlot : MonoBehaviour
{
    public GameObject effectPanel;      // 彩蛋触发效果面板
    [TextArea] public string eggDescription;
    public TMP_Text eggTextUI;

    private bool unlocked = true; // 最终彩蛋一般默认可点

    public void OnClick()
    {
        if (!unlocked) return;

        if (effectPanel != null && eggTextUI != null)
        {
            effectPanel.SetActive(true);
            eggTextUI.text = eggDescription;
        }

        Debug.Log("彩蛋被点击！");
        // TODO: 播放音效、动画等
    }

    public void CloseEffect()
    {
        if (effectPanel != null)
            effectPanel.SetActive(false);
    }
}
