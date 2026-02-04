using UnityEngine;
public class Obstacle : MonoBehaviour
{
    private WordBlockView view;
    public ColorType colorType = ColorType.Red;  // 障碍物颜色
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = GetColor(colorType);

        view = GetComponent<WordBlockView>();

        if (view == null)
        Debug.LogError("WordBlockView missing!");
        
        UpdateColor();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet == null) return;

        // 黑色子弹 →不消除
        if (bullet.colorType == ColorType.Black)
        {
            Destroy(bullet.gameObject);
            return;
        }

        // 同色 → 消失
        if (bullet.colorType == colorType)
        {
            DestroyBlock();//45行
        }
        else
        {
            // 异色 → 叠加 / 混合成新颜色
            colorType = MixColors(colorType, bullet.colorType);
            UpdateColor();

            if (sr != null)
                sr.color = GetColor(colorType);//111行
        }

        // 子弹击中后销毁
        Destroy(bullet.gameObject);
    }

    void DestroyBlock()
    {
        // TODO: 可在这里播放消除动画
        Destroy(gameObject); // 消除自己

        // 找相邻色块
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, 1f); // 半径1f可调整
        foreach (var col in neighbors)
        {
            Obstacle ob = col.GetComponent<Obstacle>();
            if (ob != null && ob.colorType == colorType)
            {
                ob.DestroyBlock(); // 递归消除同色相邻块
            }
        }
    }

    // 根据游戏规则混合颜色
    ColorType MixColors(ColorType a, ColorType b)
    {
        // 基础色 → 二级混合色
        if ((a == ColorType.Red && b == ColorType.Green) || (a == ColorType.Green && b == ColorType.Red)) return ColorType.Yellow;
        if ((a == ColorType.Red && b == ColorType.Blue) || (a == ColorType.Blue && b == ColorType.Red)) return ColorType.Purple;
        if ((a == ColorType.Green && b == ColorType.Blue) || (a == ColorType.Blue && b == ColorType.Green)) return ColorType.Cyan;

        // 混合色 → 一级基础色
        if ((a == ColorType.Yellow && b == ColorType.Purple) || (a == ColorType.Purple && b == ColorType.Yellow)) return ColorType.Red;
        if ((a == ColorType.Yellow && b == ColorType.Cyan) || (a == ColorType.Cyan && b == ColorType.Yellow)) return ColorType.Green;
        if ((a == ColorType.Purple && b == ColorType.Cyan) || (a == ColorType.Cyan && b == ColorType.Purple)) return ColorType.Blue;

        // 白色 + 任意颜色 → 保留对方颜色
        if (a == ColorType.White) return b;
        if (b == ColorType.White) return a;

        // 基础色 + 混合色（非相对色） → 保留基础色
        if (IsBase(a) && IsMix(b))
        {
            if (IsRelative(a, b)) return ColorType.White; // 相对色生成白色
            else return a; // 非相对色保留基础色
        }
        if (IsBase(b) && IsMix(a))
        {
            if (IsRelative(b, a)) return ColorType.White;
            else return b;
        }
        if (a == ColorType.Black) return ColorType.Black;
        // 默认保留 a
        return a;
    }

    void UpdateColor()
{
    if (view != null)
        view.ApplyColor(GetColor(colorType));
}

    bool IsBase(ColorType c) => c == ColorType.Red || c == ColorType.Green || c == ColorType.Blue;
    bool IsMix(ColorType c) => c == ColorType.Yellow || c == ColorType.Purple || c == ColorType.Cyan;

    bool IsRelative(ColorType baseColor, ColorType mixColor)
    {
        // 红↔天蓝，绿↔紫，蓝↔黄
        if (baseColor == ColorType.Red && mixColor == ColorType.Cyan) return true;
        if (baseColor == ColorType.Green && mixColor == ColorType.Purple) return true;
        if (baseColor == ColorType.Blue && mixColor == ColorType.Yellow) return true;
        return false;
    }

    Color GetColor(ColorType type)
    {
        switch (type)
        {
            case ColorType.Red: return Color.red;
            case ColorType.Green: return Color.green;
            case ColorType.Blue: return Color.blue;
            case ColorType.Yellow: return Color.yellow;
            case ColorType.Purple: return new Color(0.5f, 0, 0.5f); // 紫色
            case ColorType.Cyan: return Color.cyan;
            case ColorType.White: return Color.white;
            case ColorType.Black: return Color.black;
            default: return Color.white;
        }
    }
}
