using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// 繧ｹ繧ｳ繧｢縺ｫ蠢懊§縺溘Λ繝ｳ繧ｯ縺斐→縺ｮ貍泌・險ｭ螳壹ｒ縺ｾ縺ｨ繧√ｋ讒矩菴・/// </summary>
[System.Serializable]
/// <summary>

public class ScoreRank
{
    [Tooltip("縺薙・繝ｩ繝ｳ繧ｯ縺ｫ縺ｪ繧九◆繧√↓蠢・ｦ√↑譛菴弱せ繧ｳ繧｢")]
    public int minScore = 0;
    
    [Tooltip("縺薙・繝ｩ繝ｳ繧ｯ縺ｮ譎ゅ・繧ｹ繧ｳ繧｢繝・く繧ｹ繝医・濶ｲ")]
    public Color scoreColor = Color.white; // 繝・ヵ繧ｩ繝ｫ繝医ｒ逋ｽ・井ｸ埼乗・・峨↓縺励※騾乗・縺ｫ縺ｪ繧九・繧帝亟縺・    
    [Tooltip("繝・く繧ｹ繝医・髴・∴蟷・ｼ・縺ｪ繧蛾怫縺医↑縺・ｼ・)]
    public float shakeAmount = 0f;
    
    [Tooltip("繝・く繧ｹ繝医・髴・∴繧ｹ繝斐・繝・)]
    public float shakeSpeed = 10f;
    
    [Tooltip("縺薙・繝ｩ繝ｳ繧ｯ縺ｧ蜃ｺ迴ｾ縺輔○繧九ヱ繝ｼ繝・ぅ繧ｯ繝ｫ繧ｨ繝輔ぉ繧ｯ繝茨ｼ井ｻｻ諢擾ｼ・)]
    public GameObject effectPrefab;
}

/// <summary>
/// 繝ｪ繧ｶ繝ｫ繝育判髱｢・・esultScene・峨・蛻ｶ蠕｡繧定｡後≧繧ｹ繧ｯ繝ｪ繝励ヨ縲・/// 繧ｹ繧ｳ繧｢縺ｫ蠢懊§縺ｦ濶ｲ縲√い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ縲√お繝輔ぉ繧ｯ繝医ｒ雎ｪ闖ｯ縺ｫ縺吶ｋ讖溯・繧定ｿｽ蜉縺励※縺・∪縺吶・/// </summary>
public class ResultScene : MonoBehaviour
{
    // --- 螳壽焚 ---
    private const string LAST_SCORE_KEY = "LastScore";
    private const string NEXT_SCENE     = "Title";
    private const string TAP_TEXT       = "Tap to Title";

    // --- UI險ｭ螳・---
    [Header("UI")]
    [Tooltip("繧ｹ繧ｳ繧｢繧定｡ｨ遉ｺ縺吶ｋ TextMeshPro (縺ゅｋ蝣ｴ蜷・")]
    [SerializeField] private TMP_Text resultScoreTextTMP;
    [Tooltip("繧ｹ繧ｳ繧｢繧定｡ｨ遉ｺ縺吶ｋ 蠕捺擂縺ｮ Text (縺ゅｋ蝣ｴ蜷・")]
    [SerializeField] private Text     resultScoreText;
    
    [Tooltip("縲卦ap to Title縲阪ｒ陦ｨ遉ｺ縺吶ｋ TextMeshPro (縺ゅｋ蝣ｴ蜷・")]
    [SerializeField] private TMP_Text tapToTitleTextTMP;
    [Tooltip("縲卦ap to Title縲阪ｒ陦ｨ遉ｺ縺吶ｋ 蠕捺擂縺ｮ Text (縺ゅｋ蝣ｴ蜷・")]
    [SerializeField] private Text     tapToTitleText;

    // --- 貍泌・險ｭ螳・---
    [Header("繝ｩ繝ｳ繧ｯ蛻･縺ｮ貍泌・險ｭ螳夲ｼ医せ繧ｳ繧｢縺碁ｫ倥＞鬆・↓荳ｦ縺ｹ縺ｪ縺上※繧り・蜍輔た繝ｼ繝医＆繧後∪縺呻ｼ・)]
    [SerializeField] private List<ScoreRank> scoreRanks = new List<ScoreRank>();

    // --- 繝輔ぉ繝ｼ繝芽ｨｭ螳・---
    [Header("繝輔ぉ繝ｼ繝芽ｨｭ螳・)]
    [SerializeField] private Image  fadeImage;
    [SerializeField] private float  fadeDuration  = 1f;
    [SerializeField] private string nextSceneName = NEXT_SCENE;

    // --- BGM險ｭ螳・---
    [Header("BGM險ｭ螳・)]
    [Tooltip("繝ｪ繧ｶ繝ｫ繝育判髱｢縺ｧ豬√☆BGM・域悴險ｭ螳壹・蝣ｴ蜷医・閾ｪ蜍輔〒ResultBGM繧定ｪｭ縺ｿ霎ｼ縺ｿ縺ｾ縺呻ｼ・)]
    [SerializeField] private AudioClip bgmClip;

    // 蜀・Κ螟画焚
    private bool isTransitioning;
    private int currentScore;
    private ScoreRank currentRank;
    private Vector2 initialScorePosition;
    private RectTransform scoreRectTransform;

    /// <summary>
    /// 繧ｷ繝ｼ繝ｳ髢句ｧ区凾縺ｮ蛻晄悄蛹門・逅・りレ譎ｯ險ｭ螳壹。GM蜀咲函縲√せ繧ｳ繧｢縺ｮ隱ｭ縺ｿ霎ｼ縺ｿ縺ｪ縺ｩ繧貞ｮ溯｡後・    /// </summary>
    private void Start()
    {
        // --- 閭梧勹逕ｻ蜒上・險ｭ螳・---
        BackgroundHelper.SetupBackground("bg_result");

        // --- BGM縺ｮ蜀咲函 ---
        if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Audio/ResultBGM");
        if (AudioManager.Instance != null && bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(bgmClip, 0.4f); // 髻ｳ驥上・0.4f・亥ｰ代＠謗ｧ縺医ａ・・        }

        // 繝輔ぉ繝ｼ繝臥畑縺ｮ逕ｻ蜒上ｒ譛蛻昴↓騾乗・縺ｫ縺励※縺翫￥
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
        }

        // --- 繧ｹ繧ｳ繧｢縺ｮ隱ｭ縺ｿ霎ｼ縺ｿ ---
        currentScore = PlayerPrefs.GetInt(LAST_SCORE_KEY, 0);
        
        // TMP縺矩壼ｸｸ縺ｮText縺ｮ縺ｩ縺｡繧峨°縺ｫ蜿ら・繧偵そ繝・ヨ繧｢繝・・縺励∝・譛溯｡ｨ遉ｺ繧・0"縺ｫ縺励※縺翫￥
        if (resultScoreTextTMP != null)
        {
            resultScoreTextTMP.text = "0";
            scoreRectTransform = resultScoreTextTMP.GetComponent<RectTransform>();
        }
        if (resultScoreText != null)
        {
            resultScoreText.text = "0";
            if (scoreRectTransform == null) scoreRectTransform = resultScoreText.GetComponent<RectTransform>();
        }

        if (scoreRectTransform != null)
        {
            initialScorePosition = scoreRectTransform.anchoredPosition;
        }

        // 繧ｿ繝・・繧剃ｿ・☆繝・く繧ｹ繝医ｒ陦ｨ遉ｺ縺吶ｋ・域怙蛻昴・騾乗・縺ｫ縺励※縺翫￥・・        if (tapToTitleTextTMP != null) 
        {
            tapToTitleTextTMP.text = TAP_TEXT;
            tapToTitleTextTMP.alpha = 0f;
        }
        if (tapToTitleText != null)    
        {
            tapToTitleText.text = TAP_TEXT;
            var c = tapToTitleText.color;
            c.a = 0f;
            tapToTitleText.color = c;
        }

        // --- 繝ｩ繝ｳ繧ｯ蛻･貍泌・縺ｮ驕ｩ逕ｨ ---
        ApplyRankEffects();

        // --- 繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ縺ｮ髢句ｧ・---
        StartCoroutine(AnimateScoreUp());
    }

    /// <summary>
    /// 繧ｹ繧ｳ繧｢縺ｫ蠢懊§縺溘Λ繝ｳ繧ｯ繧貞愛螳壹＠縲∬牡繝ｻ繧ｨ繝輔ぉ繧ｯ繝医ｒ險ｭ螳壹☆繧・    /// </summary>
    private void ApplyRankEffects()
    {
        if (scoreRanks == null || scoreRanks.Count == 0) return;

        // 隕∵ｱゅせ繧ｳ繧｢縺碁ｫ倥＞鬆・ｼ磯剄鬆・ｼ峨↓繧ｽ繝ｼ繝医＠縺ｦ縲∵擅莉ｶ繧呈ｺ縺溘☆荳逡ｪ鬮倥＞繝ｩ繝ｳ繧ｯ繧呈爾縺・        var sortedRanks = scoreRanks.OrderByDescending(r => r.minScore).ToList();

        // 繝・ヵ繧ｩ繝ｫ繝医→縺励※荳逡ｪ菴弱＞繝ｩ繝ｳ繧ｯ繧貞・繧後※縺翫￥
        currentRank = sortedRanks[sortedRanks.Count - 1];

        foreach (var rank in sortedRanks)
        {
            if (currentScore >= rank.minScore)
            {
                currentRank = rank;
                break; // 荳逡ｪ鬮倥＞繝ｩ繝ｳ繧ｯ縺瑚ｦ九▽縺九▲縺溘ｉ邨ゆｺ・            }
        }

        if (currentRank == null) return;

        Color safeColor = Palette.ColorA;
        if (currentRank.minScore > 10) safeColor = Palette.ColorB;
        if (currentRank.minScore > 30) safeColor = Palette.ColorC;
        safeColor.a = 1f;

        // 1. 濶ｲ縺ｮ螟画峩
        if (resultScoreTextTMP != null) resultScoreTextTMP.color = safeColor;
        if (resultScoreText != null)    resultScoreText.color    = safeColor;

        // 2. 繧ｨ繝輔ぉ繧ｯ繝医・逕滓・
        if (currentRank.effectPrefab != null)
        {
            // 繧ｨ繝輔ぉ繧ｯ繝医ｒ逕ｻ髱｢荳ｭ螟ｮ・医∪縺溘・迚ｹ螳壹・蝣ｴ謇・峨↓逕滓・縺吶ｋ
            Instantiate(currentRank.effectPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    /// <summary>
    /// 豈弱ヵ繝ｬ繝ｼ繝蜻ｼ縺ｰ繧後ｋ譖ｴ譁ｰ蜃ｦ逅・ゅユ繧ｭ繧ｹ繝医・謠ｺ繧後ｄ轤ｹ貊・∫判髱｢繧ｿ繝・・縺ｫ繧医ｋ繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ繧貞宛蠕｡縲・    /// </summary>
    private void Update()
    {
        // --- 繝・く繧ｹ繝医・繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・域昭繧鯉ｼ・---
        if (scoreRectTransform != null && currentRank != null && currentRank.shakeAmount > 0f)
        {
            // 譎る俣縺ｨSin豕｢縲√Λ繝ｳ繝繝繧堤ｵ・∩蜷医ｏ縺帙※豼縺励￥髴・∴縺輔○繧・            float t = Time.time * currentRank.shakeSpeed;
            float shakeX = Mathf.Sin(t) * currentRank.shakeAmount + Random.Range(-currentRank.shakeAmount, currentRank.shakeAmount) * 0.2f;
            float shakeY = Mathf.Cos(t * 1.3f) * currentRank.shakeAmount + Random.Range(-currentRank.shakeAmount, currentRank.shakeAmount) * 0.2f;

            scoreRectTransform.anchoredPosition = initialScorePosition + new Vector2(shakeX, shakeY);
        }

        // --- Tap to Title 縺ｮ轤ｹ貊・い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ ---
        float alpha = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f; // 0.0 ~ 1.0繧呈ｳ｢謇薙▽
        alpha = Mathf.Lerp(0.3f, 1.0f, alpha); // 0.3 ~ 1.0縺ｮ髢薙〒轤ｹ貊・＆縺帙ｋ
        if (tapToTitleTextTMP != null && tapToTitleTextTMP.gameObject.activeSelf) 
        {
            // 繧ｳ繝ｫ繝ｼ繝√Φ縺檎ｵゅｏ縺｣縺ｦ繧｢繝ｫ繝輔ぃ縺瑚ｨｭ螳壹＆繧後※縺九ｉ繝輔ぉ繝ｼ繝峨＆縺帙ｋ繧医≧縺ｫ縺吶ｋ縺溘ａ
            // 譛蛻昴・Start縺ｧ0f縺ｫ縺励※縺・ｋ縺後√い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ髢句ｧ句ｾ後・縺薙・Update縺ｧ荳頑嶌縺阪＆繧後ｋ
            if (tapToTitleTextTMP.alpha > 0.1f || alpha > 0.1f)
                tapToTitleTextTMP.alpha = alpha;
        }
        if (tapToTitleText != null && tapToTitleText.gameObject.activeSelf)
        {
            var c = tapToTitleText.color;
            if (c.a > 0.1f || alpha > 0.1f)
            {
                c.a = alpha;
                tapToTitleText.color = c;
            }
        }

        // --- 繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ蜃ｦ逅・---
        if (!isTransitioning && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    /// <summary>
    /// 繧ｹ繧ｳ繧｢繧・縺九ｉ螳滄圀縺ｮ蛟､縺ｾ縺ｧ繧ｫ繧ｦ繝ｳ繝医い繝・・縺吶ｋ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ
    /// </summary>
    private IEnumerator AnimateScoreUp()
    {
        // Tap To Title 繧呈怙蛻昴・髫縺励※縺翫￥・・tart縺ｧ險ｭ螳壽ｸ医∩縺縺悟ｿｵ縺ｮ縺溘ａ・・        
        float duration = 0.8f; // 0.8遘偵°縺代※繧ｫ繧ｦ繝ｳ繝医い繝・・
        float elapsed = 0f;
        
        // 蟆代＠蠕・▲縺ｦ縺九ｉ繧ｫ繧ｦ繝ｳ繝磯幕蟋・        yield return new WaitForSeconds(0.2f);
        
        if (AudioManager.Instance != null)
        {
            // 繝峨Λ繝繝ｭ繝ｼ繝ｫ逧・↑髻ｳ縺後≠繧後・縺薙％縺ｧ魑ｴ繧峨☆縺ｮ繧ゅ≠繧・        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // 繧､繝ｼ繧ｺ繧｢繧ｦ繝茨ｼ亥ｾ悟濠繧・▲縺上ｊ縺ｫ縺ｪ繧具ｼ・            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            int displayScore = Mathf.RoundToInt(currentScore * easeOut);
            
            if (resultScoreTextTMP != null) resultScoreTextTMP.text = displayScore.ToString();
            if (resultScoreText != null)    resultScoreText.text    = displayScore.ToString();
            
            // 繧ｹ繧ｳ繧｢縺後き繧ｦ繝ｳ繝医い繝・・縺励※縺・ｋ髢薙∝ｰ代＠繧ｹ繧ｱ繝ｼ繝ｫ繧呈昭繧峨☆
            if (scoreRectTransform != null)
            {
                scoreRectTransform.localScale = Vector3.one * (1f + Random.Range(-0.05f, 0.05f));
            }
            
            yield return null;
        }
        
        // 譛邨ゅせ繧ｳ繧｢繧堤｢ｺ螳溘↓繧ｻ繝・ヨ
        if (resultScoreTextTMP != null) resultScoreTextTMP.text = currentScore.ToString();
        if (resultScoreText != null)    resultScoreText.text    = currentScore.ToString();
        
        // 譛蠕後↓繧ｹ繧ｱ繝ｼ繝ｫ繧偵・繝ｳ繝・→螟ｧ縺阪￥縺励※謌ｻ縺呎ｼ泌・
        if (scoreRectTransform != null)
        {
            float popDuration = 0.2f;
            float popElapsed = 0f;
            while(popElapsed < popDuration)
            {
                popElapsed += Time.deltaTime;
                float popT = popElapsed / popDuration;
                float scale = 1f;
                if (popT < 0.5f) scale = Mathf.Lerp(1f, 1.3f, popT * 2f);
                else             scale = Mathf.Lerp(1.3f, 1f, (popT - 0.5f) * 2f);
                
                scoreRectTransform.localScale = Vector3.one * scale;
                yield return null;
            }
            scoreRectTransform.localScale = Vector3.one;
        }

        // Tap To Title 縺ｮ陦ｨ遉ｺ繧定ｨｱ蜿ｯ・・pdate縺ｮ轤ｹ貊・↓蠑輔″邯吶′繧後ｋ・・        if (tapToTitleTextTMP != null) tapToTitleTextTMP.alpha = 1f;
        if (tapToTitleText != null)
        {
            var c = tapToTitleText.color;
            c.a = 1f;
            tapToTitleText.color = c;
        }
    }

    /// <summary>
    /// 繝輔ぉ繝ｼ繝峨い繧ｦ繝医＠縺ｦ縺九ｉ谺｡縺ｮ繧ｷ繝ｼ繝ｳ・医ち繧､繝医Ν・峨ｒ隱ｭ縺ｿ霎ｼ繧繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator FadeAndLoadScene()
    {
        isTransitioning = true;

        if (fadeImage != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(elapsed / fadeDuration));
                yield return null;
            }
        }

        if (SceneManager.Instance != null)
            SceneManager.Instance.LoadScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}

