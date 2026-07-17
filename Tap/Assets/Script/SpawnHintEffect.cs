using UnityEngine;

/// <summary>
/// 谺｡縺ｮCircle蜃ｺ迴ｾ菴咲ｽｮ縺ｫ荳迸ｬ陦ｨ遉ｺ縺輔ｌ繧玖埋縺・ｼｪ縺ｮ繧ｨ繝輔ぉ繧ｯ繝医・/// LineRenderer 縺ｧ蜀・ｽ｢縺ｮ繧｢繧ｦ繝医Λ繧､繝ｳ繧呈緒縺阪√☆縺舌↓繝輔ぉ繝ｼ繝峨い繧ｦ繝医＠縺ｦ閾ｪ蟾ｱ遐ｴ譽・☆繧九・///
/// 縲蝉ｽｿ縺・婿縲・/// 縺薙・繧ｹ繧ｯ繝ｪ繝励ヨ繧偵い繧ｿ繝・メ縺励◆遨ｺ縺ｮGameObject繧偵・繝ｬ繝上ヶ蛹悶＠縲・/// GameManager 縺ｮ縲郡pawn Hint Prefab縲阪ヵ繧｣繝ｼ繝ｫ繝峨↓繧｢繧ｵ繧､繝ｳ縺吶ｋ縺縺代・/// 繝槭ユ繝ｪ繧｢繝ｫ縺ｪ縺ｩ縺ｮ險ｭ螳壹・繧ｳ繝ｼ繝牙・縺ｧ閾ｪ蜍慕噪縺ｫ陦後≧縲・/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class SpawnHintEffect : MonoBehaviour
{
    // --- 霈ｪ縺ｮ繧ｵ繧､繧ｺ險ｭ螳・---
    // CirclePrefab 縺ｮ隕九◆逶ｮ縺ｮ蜊雁ｾ・↓蜷医ｏ縺帙※隱ｿ謨ｴ縺吶ｋ
    [SerializeField] private float radius    = 0.55f;
    [SerializeField] private int   segments  = 40;       // 螟壹＞縺ｻ縺ｩ貊代ｉ縺九↑蜀・↓縺ｪ繧・
    // --- 邱壹・螟ｪ縺・---
    [SerializeField] private float lineWidth = 0.04f;

    // --- 濶ｲ繝ｻ騾乗・蠎ｦ ---
    [SerializeField] private Color hintColor = new Color(1f, 1f, 1f, 0.6f);

    // --- 繝輔ぉ繝ｼ繝画凾髢難ｼ育ｧ抵ｼ・---
    [SerializeField] private float duration  = 0.4f;

    private LineRenderer lineRenderer;
    private float        elapsed;

    /// <summary>
    /// 蛻晄悄蛹門・逅・・ineRenderer縺ｮ蜿門ｾ励→險ｭ螳壹∝・縺ｮ謠冗判繧定｡後≧縲・    /// </summary>
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
        DrawCircle();
    }

    /// <summary>
    /// LineRenderer縺ｮ蜷・ｨｮ繝励Ο繝代ユ繧｣・医・繝・Μ繧｢繝ｫ縲∝､ｪ縺輔∬牡縺ｪ縺ｩ・峨ｒ險ｭ螳壹☆繧九・    /// </summary>
    private void SetupLineRenderer()
    {
        // 繝槭ユ繝ｪ繧｢繝ｫ繧偵さ繝ｼ繝峨〒險ｭ螳夲ｼ・nspector縺ｧ縺ｮ謇句虚險ｭ螳壻ｸ崎ｦ・ｼ・        lineRenderer.material         = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.loop             = true;              // 蟋狗せ縺ｨ邨らせ繧偵▽縺ｪ縺偵※髢峨§縺溷・縺ｫ縺吶ｋ

        // Palette繧剃ｽｿ縺｣縺ｦ邨ｱ荳
        hintColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.6f);

        // -- LineRenderer 蝓ｺ譛ｬ險ｭ螳・--
        lineRenderer.positionCount    = segments;
        lineRenderer.startWidth       = lineWidth;
        lineRenderer.endWidth         = lineWidth;
        lineRenderer.useWorldSpace    = false;             // 繝ｭ繝ｼ繧ｫ繝ｫ蠎ｧ讓吶〒謠冗判
        lineRenderer.startColor       = hintColor;
        lineRenderer.endColor         = hintColor;
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder     = 5;                 // Circle繧医ｊ謇句燕縺ｫ陦ｨ遉ｺ
    }

    /// <summary>
    /// 謖・ｮ壹＆繧後◆蛻・牡謨ｰ・・egments・峨↓蝓ｺ縺･縺・※縲∝・蠖｢縺ｫ縺ｪ繧九ｈ縺・↓鬆らせ繧帝・鄂ｮ縺吶ｋ縲・    /// </summary>
    private void DrawCircle()
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            float x     = Mathf.Cos(angle) * radius;
            float y     = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    /// <summary>
    /// 豈弱ヵ繝ｬ繝ｼ繝蜻ｼ縺ｰ繧後∫ｵ碁℃譎る俣縺ｫ蠢懊§縺ｦ騾乗・蠎ｦ繧剃ｸ九￡・医ヵ繧ｧ繝ｼ繝峨い繧ｦ繝茨ｼ峨∵欠螳壽凾髢鍋ｵ碁℃蠕後↓閾ｪ霄ｫ繧堤ｴ譽・☆繧九・    /// </summary>
    private void Update()
    {
        elapsed += Time.deltaTime;

        // 繝輔ぉ繝ｼ繝峨い繧ｦ繝茨ｼ啾lpha 繧・蛻晄悄蛟､竊・ 縺ｫ繝ｪ繝九い縺ｫ螟牙喧縺輔○繧・        float alpha = Mathf.Lerp(hintColor.a, 0f, elapsed / duration);
        Color c     = new Color(hintColor.r, hintColor.g, hintColor.b, alpha);
        lineRenderer.startColor = c;
        lineRenderer.endColor   = c;

        // 譎る俣縺碁℃縺弱◆繧芽・蟾ｱ遐ｴ譽・        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
