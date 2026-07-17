using System.Collections;
using UnityEngine;

/// <summary>
/// 豸医∴縺溷・縺ｮ菴咲ｽｮ縺九ｉ谺｡縺ｮ蜃ｺ迴ｾ菴咲ｽｮ縺ｸ蜷代°縺｣縺ｦ邱壹ｒ莨ｸ縺ｰ縺吶お繝輔ぉ繧ｯ繝医・/// 繧ｳ繝ｫ繝ｼ繝√Φ縺ｧ縲御ｼｸ縺ｳ繧・竊・菫晄戟 竊・繝輔ぉ繝ｼ繝峨い繧ｦ繝・竊・閾ｪ蟾ｱ遐ｴ譽・阪・3繝輔ぉ繝ｼ繧ｺ繧貞ｮ溯｡後☆繧九・///
/// 縲蝉ｽｿ縺・婿縲・/// 1. 遨ｺ縺ｮ GameObject 縺ｫ縺薙・繧ｹ繧ｯ繝ｪ繝励ヨ繧偵い繧ｿ繝・メ縺励※繝励Ξ繝上ヶ蛹悶☆繧・///    ・・ineRenderer 縺ｯ [RequireComponent] 縺ｧ閾ｪ蜍戊ｿｽ蜉縺輔ｌ繧具ｼ・/// 2. GameManager 縺ｮ縲靴onnect Line Prefab縲阪ヵ繧｣繝ｼ繝ｫ繝峨↓繧｢繧ｵ繧､繝ｳ縺吶ｋ
/// 3. 蜷・ヱ繝ｩ繝｡繝ｼ繧ｿ縺ｯ Inspector 縺ｾ縺溘・繧ｳ繝ｼ繝峨・繝・ヵ繧ｩ繝ｫ繝亥､縺ｧ隱ｿ謨ｴ縺吶ｋ
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ConnectLineEffect : MonoBehaviour
{
    // -------------------------------------------------------
    // Inspector 蜈ｬ髢九ヱ繝ｩ繝｡繝ｼ繧ｿ
    // -------------------------------------------------------

    [Header("繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ譎る俣・育ｧ抵ｼ・)]
    // 邱壹′蟋狗せ縺九ｉ邨らせ縺ｸ莨ｸ縺ｳ繧九・縺ｫ縺九°繧区凾髢・    [SerializeField] private float growDuration = 0.12f;
    // 螳悟・縺ｫ莨ｸ縺ｳ縺ｦ縺九ｉ豸医∴蟋九ａ繧九∪縺ｧ縺ｮ蠕・ｩ滓凾髢・    [SerializeField] private float holdDuration = 0.05f;
    // 繝輔ぉ繝ｼ繝峨い繧ｦ繝医↓縺九°繧区凾髢・    [SerializeField] private float fadeDuration = 0.18f;

    [Header("邱壹・繧ｹ繧ｿ繧､繝ｫ")]
    // 蟋狗せ蛛ｴ・域ｶ医∴縺溷・・峨・螟ｪ縺・    [SerializeField] private float startWidth = 0.06f;
    // 邨らせ蛛ｴ・域ｬ｡縺ｮ蜃ｺ迴ｾ菴咲ｽｮ・峨・螟ｪ縺包ｼ育ｴｰ縺上＠縺ｦ遏｢蜊ｰ縺ｮ繧医≧縺ｪ蜊ｰ雎｡縺ｫ・・    [SerializeField] private float endWidth   = 0.02f;
    // 蟋狗せ縺ｮ濶ｲ・・lpha 縺悟・譛滄乗・蠎ｦ縺ｫ縺ｪ繧具ｼ・    [SerializeField] private Color startColor = new Color(1.0f, 0.85f, 0.2f, 0.9f);
    // 邨らせ縺ｮ濶ｲ・郁埋繧√↓縺励※譁ｹ蜷第─繧貞・縺呻ｼ・    [SerializeField] private Color endColor   = new Color(1.0f, 0.85f, 0.2f, 0.3f);

    // -------------------------------------------------------
    // 蜀・Κ螟画焚
    // -------------------------------------------------------
    private LineRenderer lineRenderer;

    // -------------------------------------------------------
    // Unity 繝ｩ繧､繝輔し繧､繧ｯ繝ｫ
    // -------------------------------------------------------
    /// <summary>
    /// 襍ｷ蜍墓凾縺ｫ蜻ｼ縺ｰ繧後ｋ蜃ｦ逅・ょｿ・ｦ√↑繧ｳ繝ｳ繝昴・繝阪Φ繝医・蜿門ｾ励→蛻晄悄險ｭ螳壹ｒ陦後≧縲・    /// </summary>
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    /// <summary>
    /// LineRenderer 縺ｮ蝓ｺ譛ｬ險ｭ螳壹ｒ繧ｳ繝ｼ繝峨〒陦後≧・・nspector 縺ｧ縺ｮ謇句虚險ｭ螳壻ｸ崎ｦ・ｼ・    /// </summary>
    private void SetupLineRenderer()
    {
        // "Sprites/Default" 繧ｷ繧ｧ繝ｼ繝繝ｼ繧剃ｽｿ縺医・ alpha 縺悟渚譏縺輔ｌ繧・        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Palette繧剃ｽｿ逕ｨ縺励※濶ｲ繧堤ｵｱ荳
        startColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.9f);
        endColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.3f);

        // --- LineRenderer 縺ｮ蛻晄悄險ｭ螳・---
        lineRenderer.positionCount    = 2;
        lineRenderer.startWidth       = startWidth;
        lineRenderer.endWidth         = endWidth;
        lineRenderer.useWorldSpace    = true;   // 繝ｯ繝ｼ繝ｫ繝牙ｺｧ讓吶〒蟋狗せ繝ｻ邨らせ繧呈欠螳壹☆繧・        lineRenderer.startColor       = startColor;
        lineRenderer.endColor         = endColor;
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder     = 10;     // Circle 繧医ｊ蜑埼擇縺ｫ謠冗判縺吶ｋ
    }

    // -------------------------------------------------------
    // 蜈ｬ髢九Γ繧ｽ繝・ラ
    // -------------------------------------------------------

    /// <summary>
    /// 邱壹・繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧帝幕蟋九☆繧九・    /// GameManager.SpawnNextCircle() 蜀・Κ縺九ｉ蜻ｼ縺ｰ繧後ｋ縲・    /// </summary>
    /// <param name="from">蟋狗せ・医ち繝・・縺輔ｌ縺溷・縺ｮ蠎ｧ讓呻ｼ・/param>
    /// <param name="to">邨らせ・域ｬ｡縺ｮ蜀・・蜃ｺ迴ｾ蠎ｧ讓呻ｼ・/param>
    public void Play(Vector3 from, Vector3 to)
    {
        StartCoroutine(AnimateLine(from, to));
    }

    // -------------------------------------------------------
    // 繧ｳ繝ｫ繝ｼ繝√Φ・・繝輔ぉ繝ｼ繧ｺ縺ｮ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・・    // -------------------------------------------------------
    private IEnumerator AnimateLine(Vector3 from, Vector3 to)
    {
        // ===== 繝輔ぉ繝ｼ繧ｺ1: 邱壹ｒ from 竊・to 縺ｫ蜷代°縺｣縺ｦ莨ｸ縺ｰ縺・=====
        float elapsed = 0f;
        lineRenderer.SetPosition(0, from); // 蟋狗せ縺ｯ蝗ｺ螳・        lineRenderer.SetPosition(1, from); // 譛蛻昴・髟ｷ縺・・亥ｧ狗せ縺ｨ蜷後§菴咲ｽｮ・・
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            // EaseOut: 譛蛻昴・騾溘￥縲∫ｵらせ縺ｫ霑代▼縺上↓縺､繧後※繧・▲縺上ｊ縺ｫ縺ｪ繧・            float t = Mathf.Clamp01(elapsed / growDuration);
            t = 1f - (1f - t) * (1f - t);
            lineRenderer.SetPosition(1, Vector3.Lerp(from, to, t));
            yield return null;
        }
        lineRenderer.SetPosition(1, to); // 邨らせ繧偵・縺｣縺溘ｊ蜷医ｏ縺帙ｋ

        // ===== 繝輔ぉ繝ｼ繧ｺ2: 螳悟・縺ｫ莨ｸ縺ｳ縺溽憾諷九〒蟆代＠蠕・ｩ・=====
        yield return new WaitForSeconds(holdDuration);

        // ===== 繝輔ぉ繝ｼ繧ｺ3: alpha 繧・1竊・ 縺ｫ繝輔ぉ繝ｼ繝峨い繧ｦ繝・=====
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            // 蟋狗せ繝ｻ邨らせ縺昴ｌ縺槭ｌ縺ｮ alpha 繧貞・縺ｮ蛟､縺ｫ謗帙￠蜷医ｏ縺帙※繝輔ぉ繝ｼ繝・            lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b,
                                                startColor.a * alpha);
            lineRenderer.endColor   = new Color(endColor.r,   endColor.g,   endColor.b,
                                                endColor.a   * alpha);
            yield return null;
        }

        // 繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ螳御ｺ・ｾ後↓閾ｪ蟾ｱ遐ｴ譽・        Destroy(gameObject);
    }
}
