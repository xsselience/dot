using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public static PaintManager Instance;

    public Color CurrentColor { get; private set; } = Color.white;


    //ColorUnlockManager：管「你解锁了哪些颜色」
    //PaintManager：管「你当前选中了哪种颜色」
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrentColor(Color c)
    {
        CurrentColor = c;
    Debug.Log("当前画笔颜色 = " + c);
    }
}
