using UnityEngine;

public static class ColorCollectManager
{
    //解锁第二关用的
    public static bool HasOrange
    {
        get => PlayerPrefs.GetInt("HasOrange", 0) == 1;
        set => PlayerPrefs.SetInt("HasOrange", value ? 1 : 0);
    }

    public static bool HasGreen
    {
        get => PlayerPrefs.GetInt("HasGreen", 0) == 1;
        set => PlayerPrefs.SetInt("HasGreen", value ? 1 : 0);
    }

    public static bool HasPurple
    {
        get => PlayerPrefs.GetInt("HasPurple", 0) == 1;
        set => PlayerPrefs.SetInt("HasPurple", value ? 1 : 0);
    }

    public static bool AllCollected()
    {
        return HasOrange && HasGreen && HasPurple;
    }

    // 测试或重置用
    public static void ResetAll()
    {
        HasOrange = HasGreen = HasPurple = false;
        PlayerPrefs.Save();
    }
}
