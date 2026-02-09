using UnityEngine;
using System.Collections.Generic;

public class ColorUnlockManager : MonoBehaviour
{
    public static ColorUnlockManager Instance;

    private HashSet<ColorType> unlockedColors = new HashSet<ColorType>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //测试解锁基础颜色
        unlockedColors.Add(ColorType.Red);
        unlockedColors.Add(ColorType.Green);
        unlockedColors.Add(ColorType.Blue);
    }

    // 解锁颜色
    public void Unlock(ColorType color)
    {
        unlockedColors.Add(color);
    }

    // 是否已解锁
    public bool IsUnlocked(ColorType color)
    {
        return unlockedColors.Contains(color);
    }
}
