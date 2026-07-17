using UnityEngine;
using System.Collections;

/// <summary>
/// 繧ｲ繝ｼ繝繧偵後き繝・さ繧医￥縲阪梧ｰ玲戟縺｡繧医￥縲阪☆繧九◆繧√・貍泌・・医ず繝･繝ｼ繧ｹ・峨ｒ蜈ｨ閾ｪ蜍輔〒霑ｽ蜉縺吶ｋ繧ｯ繝ｩ繧ｹ縲・/// 繧ｫ繝｡繝ｩ縺ｫ繧｢繧ｿ繝・メ縺吶ｋ縺縺代〒縲∽ｻ･荳九・讖溯・縺瑚・蜍輔〒菴懷虚縺励∪縺吶・/// 1. 閭梧勹縺ｮ繧ｵ繧､繝舌・繧ｰ繝ｪ繝・ラ・亥虚縺冗ｶｲ逶ｮ讓｡讒假ｼ峨・閾ｪ蜍慕函謌・/// 2. 逕ｻ髱｢縺ｮ繧ｷ繧ｧ繧､繧ｯ・医し繝ｼ繧ｯ繝ｫ繧呈ｶ医＠縺滓凾・・/// 3. 閭梧勹縺ｮ繝輔Λ繝・す繝･・医し繝ｼ繧ｯ繝ｫ繧呈ｶ医＠縺滓凾・・/// </summary>
public class GameJuiceManager : MonoBehaviour
{
    public static GameJuiceManager Instance;

    [Header("繧ｰ繝ｪ繝・ラ險ｭ螳夲ｼ郁レ譎ｯ縺ｮ繧ｵ繧､繝舌・遨ｺ髢難ｼ・)]
    [Tooltip("閭梧勹繧呈ｵ√ｌ繧狗ｷ壹・濶ｲ")]
    public Color gridColor = new Color(0f, 0.8f, 1f, 0.15f); // 縺・▲縺吶ｉ蜈峨ｋ豌ｴ濶ｲ
    [Tooltip("閭梧勹縺梧ｵ√ｌ繧九せ繝斐・繝・)]
    public float scrollSpeed = 2f;

    [Header("繧ｷ繧ｧ繧､繧ｯ險ｭ螳夲ｼ医し繝ｼ繧ｯ繝ｫ繧呈ｶ医＠縺滓凾縺ｮ謠ｺ繧鯉ｼ・)]
    [Tooltip("謠ｺ繧後ｋ譎る俣・育ｧ抵ｼ・)]
    public float shakeDuration = 0.005f;
    [Tooltip("謠ｺ繧後・豼縺励＆")]
    public float shakeMagnitude = 0.1f;

    [Header("繝輔Λ繝・す繝･險ｭ螳夲ｼ医し繝ｼ繧ｯ繝ｫ繧呈ｶ医＠縺滓凾縺ｮ蜈会ｼ・)]
    [Tooltip("繝輔Λ繝・す繝･縺吶ｋ髫帙・荳迸ｬ縺ｮ閭梧勹濶ｲ")]
    public Color flashColor = new Color(0.2f, 0.2f, 0.4f); // 蟆代＠譏弱ｋ縺・搨

    private Camera cam;
    private Vector3 originalCameraPos;
    private Color originalBackgroundColor;
    private GameObject gridContainer;

    /// <summary>
    /// 襍ｷ蜍墓凾縺ｫ蜻ｼ縺ｰ繧後ｋ蛻晄悄蛹門・逅・ゅす繝ｳ繧ｰ繝ｫ繝医Φ縺ｮ險ｭ螳壹→繧ｫ繝｡繝ｩ諠・ｱ縺ｮ菫晏ｭ倥ｒ陦後≧縲・    /// </summary>
    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            originalCameraPos = cam.transform.localPosition;
            originalBackgroundColor = cam.backgroundColor;
        }

        // Palette繧剃ｽｿ縺｣縺ｦ濶ｲ繧堤ｵｱ荳
        gridColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.15f);
        flashColor = new Color(Palette.ColorA.r * 0.5f, Palette.ColorA.g * 0.5f, Palette.ColorA.b * 0.5f); // 證励ａ縺ｮ蜷檎ｳｻ濶ｲ

        // --- 繧ｵ繧､繝舌・縺ｪ閭梧勹繧ｰ繝ｪ繝・ラ繧定・蜍慕函謌・---
        // 閭梧勹逕ｻ蜒上ｒ霑ｽ蜉縺励◆縺溘ａ縲∫ｷ夲ｼ医げ繝ｪ繝・ラ・峨・謠冗判縺励↑縺・ｈ縺・↓辟｡蜉ｹ蛹・        // GenerateCyberGrid();
    }

    /// <summary>
    /// 繝励Ο繧ｰ繝ｩ繝縺ｧLineRenderer繧剃ｽｿ縺｣縺ｦ邯ｲ逶ｮ讓｡讒倥ｒ謠冗判縺励∪縺・    /// </summary>
    private void GenerateCyberGrid()
    {
        gridContainer = new GameObject("CyberGrid");
        gridContainer.transform.SetParent(transform); // 繧ｫ繝｡繝ｩ縺ｮ蟄舌が繝悶ず繧ｧ繧ｯ繝医↓縺吶ｋ
        gridContainer.transform.localPosition = new Vector3(0, 0, 15f); // 繧ｫ繝｡繝ｩ縺ｮ螂･縺ｫ驟咲ｽｮ

        // 2D讓呎ｺ悶・繧ｷ繝ｳ繝励Ν縺ｪ繝槭ユ繝ｪ繧｢繝ｫ繧剃ｽｿ逕ｨ
        Material lineMat = new Material(Shader.Find("Sprites/Default"));
        int gridSize = 15;
        float spacing = 2.0f;

        // 邵ｦ邱壹→讓ｪ邱壹ｒ逕滓・
        for (int i = -gridSize; i <= gridSize; i++)
        {
            // 邵ｦ
            CreateLine(new Vector3(i * spacing, -gridSize * spacing, 0), new Vector3(i * spacing, gridSize * spacing, 0), lineMat);
            // 讓ｪ
            CreateLine(new Vector3(-gridSize * spacing, i * spacing, 0), new Vector3(gridSize * spacing, i * spacing, 0), lineMat);
        }
    }

    /// <summary>
    /// 謖・ｮ壹＠縺・轤ｹ髢薙↓邱壹ｒ逕滓・縺励※繧ｰ繝ｪ繝・ラ縺ｮ荳驛ｨ縺ｨ縺励※謠冗判縺吶ｋ縲・    /// </summary>
    private void CreateLine(Vector3 start, Vector3 end, Material mat)
    {
        GameObject line = new GameObject("GridLine");
        line.transform.SetParent(gridContainer.transform, false);
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = mat;
        lr.startColor = gridColor;
        lr.endColor = gridColor;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.sortingOrder = -100; // 閭梧勹縺ｫ謠冗判
    }

    /// <summary>
    /// 豈弱ヵ繝ｬ繝ｼ繝蜻ｼ縺ｰ繧後ｋ蜃ｦ逅・りレ譎ｯ縺ｮ繧ｰ繝ｪ繝・ラ繧偵せ繧ｯ繝ｭ繝ｼ繝ｫ縺輔○縺ｦ繧ｹ繝斐・繝画─繧貞・縺吶・    /// </summary>
    private void Update()
    {
        // 繧ｰ繝ｪ繝・ラ繧偵せ繧ｯ繝ｭ繝ｼ繝ｫ縺輔○縺ｦ繧ｹ繝斐・繝画─繧貞・縺・        if (gridContainer != null)
        {
            gridContainer.transform.localPosition += Vector3.down * scrollSpeed * Time.deltaTime;
            
            // 荳螳夊ｷ晞屬騾ｲ繧薙□繧牙・縺ｫ謌ｻ縺呻ｼ育┌髯舌せ繧ｯ繝ｭ繝ｼ繝ｫ・・            if (gridContainer.transform.localPosition.y <= -2.0f)
            {
                gridContainer.transform.localPosition += Vector3.up * 2.0f;
            }
        }
    }

    /// <summary>
    /// 繧ｵ繝ｼ繧ｯ繝ｫ繧呈ｶ医＠縺滓凾縺ｫ蜻ｼ縺ｶ縺ｨ縲∫判髱｢縺梧昭繧後※蜈峨ｋ・√さ繝ｳ繝懊↓蠢懊§縺ｦ豼縺励￥縺ｪ繧九・    /// </summary>
    public void PlayHitEffect(int combo = 0)
    {
        StopAllCoroutines();
        if (cam != null)
        {
            StartCoroutine(ShakeRoutine(combo));
            StartCoroutine(FlashRoutine());
        }
    }

    /// <summary>
    /// 逕ｻ髱｢繧呈昭繧峨☆繧ｳ繝ｫ繝ｼ繝√Φ縲ゅさ繝ｳ繝懈焚縺ｫ蠢懊§縺ｦ謠ｺ繧後ｒ螟ｧ縺阪￥縺吶ｋ縲・    /// </summary>
    private IEnumerator ShakeRoutine(int combo)
    {
        // 繧ｳ繝ｳ繝懈焚縺ｫ蠢懊§縺ｦ譛螟ｧ2蛟阪∪縺ｧ謠ｺ繧後ｒ螟ｧ縺阪￥縺吶ｋ
        float comboMultiplier = 1f + Mathf.Min(combo, 20) * 0.005f;
        float currentShakeDuration = shakeDuration * comboMultiplier;
        float currentShakeMagnitude = shakeMagnitude * comboMultiplier;

        float elapsed = 0f;
        
        // 繝ｩ繝ｳ繝繝縺ｪ髢句ｧ九が繝輔そ繝・ヨ繧定ｨｭ縺代ｋ縺薙→縺ｧ縲∵ｯ主屓驕輔≧謠ｺ繧梧婿縺ｫ縺吶ｋ
        float randomOffsetX = Random.Range(0f, 100f);
        float randomOffsetY = Random.Range(0f, 100f);

        while (elapsed < currentShakeDuration)
        {
            elapsed += Time.deltaTime;
            
            // 蠕悟濠縺ｫ縺ｪ繧九↓縺､繧後※謠ｺ繧後ｒ貊代ｉ縺九↓貂幄｡ｰ・医ヵ繧ｧ繝ｼ繝峨い繧ｦ繝茨ｼ峨＆縺帙ｋ
            float damping = 1f - (elapsed / currentShakeDuration);
            float currentMag = currentShakeMagnitude * damping;
            
            // Random.Range縺ｮ莉｣繧上ｊ縺ｫPerlinNoise繧剃ｽｿ縺・％縺ｨ縺ｧ縲√ぎ繧ｿ繧ｬ繧ｿ縺帙★貊代ｉ縺九↓謠ｺ繧峨☆
            float noiseX = Mathf.PerlinNoise(randomOffsetX + Time.time * 25f, 0f) * 2f - 1f;
            float noiseY = Mathf.PerlinNoise(0f, randomOffsetY + Time.time * 25f) * 2f - 1f;
            
            float x = noiseX * currentMag;
            float y = noiseY * currentMag;
            
            cam.transform.localPosition = new Vector3(originalCameraPos.x + x, originalCameraPos.y + y, originalCameraPos.z);
            yield return null;
        }
        cam.transform.localPosition = originalCameraPos;
    }

    /// <summary>
    /// 逕ｻ髱｢繧偵ヵ繝ｩ繝・す繝･縺輔○繧九さ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator FlashRoutine()
    {
        cam.backgroundColor = flashColor;
        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            cam.backgroundColor = Color.Lerp(flashColor, originalBackgroundColor, elapsed / 0.15f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.backgroundColor = originalBackgroundColor;
    }

    /// <summary>
    /// 謖・ｮ壹＠縺溷ｺｧ讓吶↓譁・ｭ励ｒ豬ｮ縺九・荳翫′繧峨○繧具ｼ医ヵ繝ｭ繝ｼ繝・ぅ繝ｳ繧ｰ繝・く繧ｹ繝茨ｼ・    /// </summary>
    public void SpawnFloatingText(Vector3 position, string text, Color color, float scale = 1f)
    {
        // 譁ｰ縺励＞GameObject繧剃ｽ懈・
        GameObject floatObj = new GameObject("FloatingText");
        floatObj.transform.position = position;
        floatObj.transform.localScale = Vector3.one * scale;
        // TextMeshPro縺ｮ霑ｽ蜉縺ｨ險ｭ螳・        TMPro.TextMeshPro tmp = floatObj.AddComponent<TMPro.TextMeshPro>();
        tmp.text = text;
        tmp.color = color;
        tmp.fontSize = 8f;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.fontStyle = TMPro.FontStyles.Bold;
        // 謠冗判鬆・ｒ譛蜑埼擇縺ｫ縺吶ｋ
        tmp.sortingOrder = 100;

        // 繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ縺ｮ繧ｳ繝ｫ繝ｼ繝√Φ繧帝幕蟋・        StartCoroutine(FloatingTextRoutine(floatObj, tmp));
    }

    private IEnumerator FloatingTextRoutine(GameObject obj, TMPro.TextMeshPro tmp)
    {
        float duration = 0.8f;
        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f; // 荳翫↓遘ｻ蜍・        Color startColor = tmp.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // 騾乗・縺ｫ

        while (elapsed < duration)
        {
            if (obj == null) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 繧､繝ｼ繧ｺ繧｢繧ｦ繝茨ｼ亥ｾ舌・↓驕・￥縺ｪ繧具ｼ・            float easeOut = 1f - (1f - t) * (1f - t);
            
            obj.transform.position = Vector3.Lerp(startPos, endPos, easeOut);
            tmp.color = Color.Lerp(startColor, endColor, easeOut);
            
            yield return null;
        }
        
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
