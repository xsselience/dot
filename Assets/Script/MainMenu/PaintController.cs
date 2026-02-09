using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// PaintController (双层方案)
/// - 底层 paintTexture 存涂色 (透明背景)
/// - 上层 lineArtImage 用于显示线稿（可用材质），但逻辑从 processedLineArtTexture 读取
/// - 在 InitCanvas() 会生成 processedLineArtTexture（把白色 -> 透明）
/// - FloodFill 使用 processedLineArtTexture 作为边界（alpha > threshold 表示线条）
/// </summary>
public class PaintController : MonoBehaviour
{
    [Header("画布尺寸")]
    public int textureWidth = 512;
    public int textureHeight = 512;

    [Header("UI")]
    public RawImage paintImage;   // 底层画布显示 (RenderTexture)
    public RawImage lineArtImage; // 上层线稿用于显示（可用 shader）

    [Header("线稿透明化阈值 - 大于则认为是白色背景")]
    [Range(0f, 1f)] public float whiteThreshold = 0.9f;

    [Header("线条识别阈值 - alpha 或灰度小于视为线条")]
    [Range(0f, 1f)] public float lineThreshold = 0.1f;

    Texture2D paintTexture;             // 底层可写纹理（我们改像素）
    RenderTexture renderTexture;        // 用于显示的 RT
    Texture2D processedLineArtTexture;  // 线稿的可读副本（白->透明处理过）, 用作 mask

    private bool eraseMode = false;

    void Start()
    {
        InitCanvas();
    }

    void InitCanvas()
    {
        // --- 1. 创建底层 paintTexture（透明背景） ---
        paintTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        paintTexture.filterMode = FilterMode.Point;

        Color clear = new Color(0, 0, 0, 0); // 透明背景
        for (int x = 0; x < textureWidth; x++)
            for (int y = 0; y < textureHeight; y++)
                paintTexture.SetPixel(x, y, clear);
        paintTexture.Apply();

        // --- 2. 初始化 RenderTexture 显示底层画布 ---
        renderTexture = new RenderTexture(textureWidth, textureHeight, 0);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();
        Graphics.Blit(paintTexture, renderTexture);
        paintImage.texture = renderTexture;
        paintImage.color = Color.white;

        // --- 3. 获取并处理 lineArtImage 的纹理，生成 processedLineArtTexture ---
        if (lineArtImage != null && lineArtImage.texture != null)
        {
            Texture2D src = GetReadableTextureFromRawImage(lineArtImage);
            if (src != null)
            {
                processedLineArtTexture = ProcessLineArtMakeTransparency(src, whiteThreshold);
            }
            else
            {
                Debug.LogWarning("PaintController: 无法获取 lineArt 可读纹理，边界检测将不可用。");
                processedLineArtTexture = null;
            }
        }
        else
        {
            processedLineArtTexture = null;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryFloodFill();
        }
    }

    void TryFloodFill()
    {
        Vector2 localPos;
        RectTransform rt = paintImage.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out localPos))
            return;

        Rect r = rt.rect;
        float u = (localPos.x - r.x) / r.width; // 0..1
        float v = (localPos.y - r.y) / r.height;

        int x = Mathf.FloorToInt(u * textureWidth);
        int y = Mathf.FloorToInt(v * textureHeight);

        if (x < 0 || x >= textureWidth || y < 0 || y >= textureHeight) return;

        // 如果有处理后的线稿 mask，用它判断点击处是否线条或可填充
        if (processedLineArtTexture != null)
        {
            // 注意坐标映射： processedLineArtTexture 可能与 paintTexture 尺寸不同，
            // 使用 GetPixelBilinear 以 u,v 归一化坐标采样
            Color lineSample = processedLineArtTexture.GetPixelBilinear((x + 0.5f) / textureWidth, (y + 0.5f) / textureHeight);
            if (lineSample.a > lineThreshold)
            {
                // 点击到线条（或不可填充处）——直接返回
                return;
            }
        }

        Color fillColor = eraseMode ? new Color(0, 0, 0, 0) : PaintManager.Instance.CurrentColor;
        FloodFill(x, y, fillColor);
    }

    // ================= 洪水填充（受 processedLineArtTexture 边界控制） =================
    void FloodFill(int startX, int startY, Color newColor)
    {
        // 基本保护
        if (startX < 0 || startX >= textureWidth || startY < 0 || startY >= textureHeight) return;

        Color targetColor = paintTexture.GetPixel(startX, startY);

        // 如果要填充的点是线稿（mask），则不填
        if (processedLineArtTexture != null)
        {
            Color lineSample = processedLineArtTexture.GetPixelBilinear((startX + 0.5f) / textureWidth, (startY + 0.5f) / textureHeight);
            if (lineSample.a > lineThreshold) return;
        }

        if (SameColor(targetColor, newColor)) return;

        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(new Vector2Int(startX, startY));

        while (q.Count > 0)
        {
            Vector2Int p = q.Dequeue();
            int px = p.x, py = p.y;

            if (px < 0 || px >= textureWidth || py < 0 || py >= textureHeight) continue;

            Color cur = paintTexture.GetPixel(px, py);

            // 若当前像素已不是 targetColor，则跳过（保持 floodFill 标准逻辑）
            if (!SameColor(cur, targetColor)) continue;

            // 若这像素在线稿上（mask），则跳过（阻止扩展穿过线条）
            if (processedLineArtTexture != null)
            {
                Color lineSample = processedLineArtTexture.GetPixelBilinear((px + 0.5f) / textureWidth, (py + 0.5f) / textureHeight);
                if (lineSample.a > lineThreshold) continue;
            }

            // 设置颜色
            paintTexture.SetPixel(px, py, newColor);

            q.Enqueue(new Vector2Int(px + 1, py));
            q.Enqueue(new Vector2Int(px - 1, py));
            q.Enqueue(new Vector2Int(px, py + 1));
            q.Enqueue(new Vector2Int(px, py - 1));
        }

        paintTexture.Apply();
        Graphics.Blit(paintTexture, renderTexture);
    }

    bool SameColor(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.01f &&
               Mathf.Abs(a.g - b.g) < 0.01f &&
               Mathf.Abs(a.b - b.b) < 0.01f &&
               Mathf.Abs(a.a - b.a) < 0.01f;
    }

    public void ClearAll()
    {
        Color clear = new Color(0, 0, 0, 0);
        for (int x = 0; x < textureWidth; x++)
            for (int y = 0; y < textureHeight; y++)
                paintTexture.SetPixel(x, y, clear);
        paintTexture.Apply();
        Graphics.Blit(paintTexture, renderTexture);
    }

    public void EnterEraseMode() => eraseMode = true;
    public void ExitEraseMode() => eraseMode = false;

    // ---------------------- 辅助：把 RawImage 的 texture 变成可读的 Texture2D 副本 ----------------------
    // 兼容：Texture2D(可读或不可读)、RenderTexture 等
    private Texture2D GetReadableTextureFromRawImage(RawImage raw)
    {
        if (raw == null || raw.texture == null) return null;

        // 如果底图就是 Texture2D 并且是可读的，尝试直接返回它
        Texture2D tex2D = raw.texture as Texture2D;
        if (tex2D != null)
        {
            try
            {
                tex2D.GetPixel(0, 0); // 若抛异常则说明不可读
                return tex2D;
            }
            catch (UnityException)
            {
                // 不可读，继续走复制流程
            }
        }

        // 否则用 RenderTexture + ReadPixels 复制一份
        RenderTexture prev = RenderTexture.active;
        int w = raw.texture.width;
        int h = raw.texture.height;
        RenderTexture tmp = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
        tmp.filterMode = FilterMode.Point;

        Graphics.Blit(raw.texture, tmp);

        RenderTexture.active = tmp;
        Texture2D copy = new Texture2D(w, h, TextureFormat.RGBA32, false);
        copy.filterMode = FilterMode.Point;
        copy.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        copy.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(tmp);

        return copy;
    }

    // ---------------------- 辅助：把线稿白色区域变透明（返回新贴图） ----------------------
    private Texture2D ProcessLineArtMakeTransparency(Texture2D src, float whiteThresh)
    {
        if (src == null) return null;
        int w = textureWidth;
        int h = textureHeight;

        // 创建目标贴图，尺寸使用 paintTexture 的尺寸，便于后续用同一坐标系
        Texture2D dst = new Texture2D(w, h, TextureFormat.RGBA32, false);
        dst.filterMode = FilterMode.Point;

        // 通过 GetPixelBilinear 从 src 采样并写入 dst
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float u = (x + 0.5f) / w;
                float v = (y + 0.5f) / h;
                Color s = src.GetPixelBilinear(u, v);

                // 判断是否接近白色（背景）——如果是则 alpha=0，否则保留颜色并 alpha=1
                float brightness = (s.r + s.g + s.b) / 3f;
                if (brightness > whiteThresh)
                {
                    // 背景 -> 透明
                    dst.SetPixel(x, y, new Color(0, 0, 0, 0f));
                }
                else
                {
                    // 线条保留为不透明（保持其颜色）
                    // 我们将线条颜色存到 dst 的 alpha 通道作为标识（也保留 rgb）
                    Color keep = new Color(s.r, s.g, s.b, 1f);
                    dst.SetPixel(x, y, keep);
                }
            }
        }

        dst.Apply();
        return dst;
    }
}
