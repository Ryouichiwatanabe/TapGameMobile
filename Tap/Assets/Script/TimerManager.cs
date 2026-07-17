using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 繧ｿ繧､繝槭・縺ｮ險域ｸｬ縺ｨUI陦ｨ遉ｺ繧呈球蠖薙☆繧九け繝ｩ繧ｹ縲・/// 谿九ｊ譎る俣縺梧ｸ帙ｋ縺ｫ縺､繧後※縲∬ｨｭ螳壹＠縺溘げ繝ｩ繝・・繧ｷ繝ｧ繝ｳ・・radient・峨↓蠕薙▲縺ｦ繝舌・縺ｮ濶ｲ縺梧ｻ代ｉ縺九↓螟牙喧縺励∪縺吶・///
/// 縲蝕nspector縺ｧ縺ｮ險ｭ螳壽婿豕輔・/// 1. timerImage: Image縺ｮ縲栗mage Type縲阪ｒ縲熊illed縲阪↓縺励√熊ill Method縲阪ｒ縲粂orizontal縲咲ｭ峨↓險ｭ螳壹＠縺欟I繧ｪ繝悶ず繧ｧ繧ｯ繝医ｒ繧｢繧ｵ繧､繝ｳ縲・/// 2. colorGradient: 繧ｿ繧､繝槭・縺ｮ谿九ｊ譎る俣縺ｫ蠢懊§縺溯牡縺ｮ螟牙喧繧定ｨｭ螳壹＠縺ｾ縺吶ゑｼ井ｾ具ｼ壼承遶ｯ(1.0)繧呈ｰｴ濶ｲ縲∝ｷｦ遶ｯ(0.0)繧定ｵ､濶ｲ・・/// 3. timerTextTMP / timerText (莉ｻ諢・: 謨ｰ蛟､縺ｧ繧り｡ｨ遉ｺ縺励◆縺・ｴ蜷医・繝・く繧ｹ繝医ｒ繧｢繧ｵ繧､繝ｳ縲・/// </summary>
public class TimerManager : MonoBehaviour
{
    // --- 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ・医←縺薙°繧峨〒繧らｰ｡蜊倥↓繧｢繧ｯ繧ｻ繧ｹ縺ｧ縺阪ｋ繧医≧縺ｫ縺吶ｋ・・---
    public static TimerManager Instance { get; private set; }

    // --- 螳壽焚 ---
    private const string TIME_PREFIX = "Time: ";

    // --- 繧ｿ繧､繝槭・蝓ｺ譛ｬ險ｭ螳・---
    [Header("繧ｿ繧､繝槭・險ｭ螳・)]
    [Tooltip("繧ｲ繝ｼ繝縺ｮ蛻ｶ髯先凾髢難ｼ育ｧ抵ｼ・)]
    [SerializeField] private float timeLimit = 30.0f;

    // --- 繧ｿ繧､繝槭・UI險ｭ螳・---
    [Header("繧ｿ繧､繝槭・UI (Fill Amount蟇ｾ蠢・")]
    [Tooltip("谿九ｊ譎る俣縺ｫ蠢懊§縺ｦ貂帙▲縺ｦ縺・￥繝舌・縺ｮImage繧ｳ繝ｳ繝昴・繝阪Φ繝・)]
    [SerializeField] private Image timerImage;
    
    [Tooltip("谿九ｊ譎る俣縺ｫ蠢懊§縺溯牡縺ｮ螟牙喧縲ょ承遶ｯ(1.0)縺梧ｺ繧ｿ繝ｳ譎ゅ∝ｷｦ遶ｯ(0.0)縺梧凾髢灘・繧梧凾縺ｮ濶ｲ縺ｫ縺ｪ繧翫∪縺・)]
    [SerializeField] private Gradient colorGradient;

    // --- 繧ｿ繧､繝槭・繝・く繧ｹ繝・I (繧ｪ繝励す繝ｧ繝ｳ) ---
    [Header("繧ｿ繧､繝槭・繝・く繧ｹ繝・I (繧ｪ繝励す繝ｧ繝ｳ)")]
    [SerializeField] private TMPro.TMP_Text timerTextTMP;
    [SerializeField] private Text     timerText;

    // --- 髴・∴繧九い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ險ｭ螳・---
    [Header("繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ險ｭ螳・)]
    [Tooltip("髴・∴縺輔○縺溘＞UI縺ｮTransform・医ヰ繝ｼ繧・ユ繧ｭ繧ｹ繝医・隕ｪ繧ｪ繝悶ず繧ｧ繧ｯ繝医↑縺ｩ・・)]
    [SerializeField] private Transform timerContainer;
    [Tooltip("谿九ｊ菴慕ｧ剃ｻ･荳九↓縺ｪ縺｣縺溘ｉ髴・∴蟋九ａ繧九°")]
    [SerializeField] private float shakeThreshold = 5f;
    [Tooltip("髴・∴繧区ｿ縺励＆・域険蟷・ｼ・)]
    [SerializeField] private float shakeAmount = 3f;

    // --- 蜀・Κ迥ｶ諷・---
    private float currentTime;
    private bool  isRunning;
    private Vector2 initialContainerPosition;
    private RectTransform containerRectTransform;

    // 繧ｿ繧､繝繧｢繝・・譎ゅ↓蜻ｼ縺ｶ繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ・・ameManager 縺九ｉ逋ｻ骭ｲ縺吶ｋ・・    private System.Action onTimeUp;

    /// <summary>谿九ｊ譎る俣繧貞､夜Κ縺九ｉ隱ｭ縺ｿ蜿悶ｊ蟆ら畑縺ｧ蜈ｬ髢九☆繧九・/summary>
    public float CurrentTime => currentTime;

    /// <summary>
    /// 繧ｪ繝悶ず繧ｧ繧ｯ繝育函謌先凾縺ｮ蛻晄悄蛹門・逅・ゅす繝ｳ繧ｰ繝ｫ繝医Φ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ縺ｮ險ｭ螳壹→繧ｰ繝ｩ繝・・繧ｷ繝ｧ繝ｳ繧ｫ繝ｩ繝ｼ縺ｮ蜿門ｾ励ｒ陦後≧縲・    /// </summary>
    private void Awake()
    {
        // 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ縺ｮ險ｭ螳・        if (Instance == null)
        {
            Instance = this;
        }

        // Palette繧剃ｽｿ縺｣縺ｦ濶ｲ繧堤ｵｱ荳
        colorGradient = Palette.GetTimerGradient();
    }

    private Image[] borderImages = new Image[4];
    private Sprite whiteSprite;
    private RectTransform borderContainerRect; // 譫邱壹ｒ謠ｺ繧峨☆縺溘ａ縺ｮ隕ｪ繧ｳ繝ｳ繝・リ
    private Vector2 initialBorderPosition;

    /// <summary>
    /// 繧ｲ繝ｼ繝髢句ｧ区凾縺ｮ蛻晄悄蛹門・逅・６I縺ｮ蛻晄悄菴咲ｽｮ菫晏ｭ倥→譫邱壹ち繧､繝槭・UI縺ｮ逕滓・繧定｡後≧縲・    /// </summary>
    private void Start()
    {
        // UI縺ｮ蛻晄悄菴咲ｽｮ繧偵ご繝ｼ繝髢句ｧ狗峩蠕後↓菫晏ｭ倥＠縺ｦ縺翫￥
        if (timerContainer != null)
        {
            containerRectTransform = timerContainer.GetComponent<RectTransform>();
            if (containerRectTransform != null)
            {
                initialContainerPosition = containerRectTransform.anchoredPosition;
            }
            else
            {
                // RectTransform縺後↑縺・ｴ蜷茨ｼ磯壼ｸｸ縺ｯ縺ｪ縺・′蠢ｵ縺ｮ縺溘ａ・・                initialContainerPosition = timerContainer.localPosition;
            }
        }

        GenerateFrameTimerUI();
    }

    /// <summary>
    /// 逕ｻ髱｢縺ｮ蝗幄ｾｺ繧貞峇繧譫邱夂憾縺ｮ繧ｿ繧､繝槭・UI繧貞虚逧・↓逕滓・繝ｻ驟咲ｽｮ縺吶ｋ縲・    /// </summary>
    private void GenerateFrameTimerUI()
    {
        // 莉･蜑阪・繧ｿ繧､繝槭・UI縺後≠繧後・髱櫁｡ｨ遉ｺ縺ｫ縺吶ｋ
        if (timerImage != null)
        {
            timerImage.gameObject.SetActive(false);
        }

        GameObject canvasObj = new GameObject("FrameTimerCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // 譛蜑埼擇縺ｫ陦ｨ遉ｺ縺励※譫縺ｫ縺吶ｋ

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        // 繝懊・繝繝ｼ繧偵∪縺ｨ繧√ｋ繧ｳ繝ｳ繝・リ繧剃ｽ懈・・医％縺ｮ繧ｳ繝ｳ繝・リ繧呈昭繧峨☆・・        GameObject borderContainerObj = new GameObject("BorderContainer");
        borderContainerRect = borderContainerObj.AddComponent<RectTransform>();
        borderContainerRect.SetParent(canvasObj.transform, false);
        borderContainerRect.anchorMin = Vector2.zero;
        borderContainerRect.anchorMax = Vector2.one;
        borderContainerRect.sizeDelta = Vector2.zero;
        borderContainerRect.anchoredPosition = Vector2.zero;
        initialBorderPosition = borderContainerRect.anchoredPosition;

        whiteSprite = CreateWhiteSprite();
        float thickness = 40f; // 逶ｮ遶九▽繧医≧縺ｫ螟ｪ繧√・譫邱壹↓縺吶ｋ

        // 譫縺ｮ邱夲ｼ井ｸ翫∝承縲∽ｸ九∝ｷｦ縺ｮ鬆・ｼ・        // 譎りｨ亥屓繧翫↓貂帙▲縺ｦ縺・￥繧医≧縺ｫ險ｭ螳夲ｼ域ｮ九ｊ縺悟ｷｦ荳翫・隗偵↓蜷代°縺｣縺ｦ豸医∴縺ｦ縺・￥・・        borderImages[0] = CreateBorder(borderContainerObj.transform, "TopBorder", 
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, thickness), 
            Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Right); 
        
        borderImages[1] = CreateBorder(borderContainerObj.transform, "RightBorder", 
            new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f), new Vector2(thickness, 0), 
            Image.FillMethod.Vertical, (int)Image.OriginVertical.Bottom);

        borderImages[2] = CreateBorder(borderContainerObj.transform, "BottomBorder", 
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, thickness), 
            Image.FillMethod.Horizontal, (int)Image.OriginHorizontal.Left); 

        borderImages[3] = CreateBorder(borderContainerObj.transform, "LeftBorder", 
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(thickness, 0), 
            Image.FillMethod.Vertical, (int)Image.OriginVertical.Top); 
    }

    /// <summary>
    /// 譫邱啅I繧呈ｧ区・縺吶ｋ蛟九・・Image・医・繝ｼ繝繝ｼ・峨ｒ逕滓・縺励∵欠螳壹＆繧後◆險ｭ螳夲ｼ医い繝ｳ繧ｫ繝ｼ縲√ヴ繝懊ャ繝医↑縺ｩ・峨ｒ驕ｩ逕ｨ縺吶ｋ縲・    /// </summary>
    private Image CreateBorder(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Image.FillMethod fillMethod, int fillOrigin)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        
        img.sprite = whiteSprite; // Fill繧呈ｩ溯・縺輔○繧九↓縺ｯ繧ｹ繝励Λ繧､繝医′蠢・・        img.type = Image.Type.Filled;
        img.fillMethod = fillMethod;
        img.fillOrigin = fillOrigin;
        img.fillAmount = 1f;

        RectTransform rect = img.rectTransform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.sizeDelta = sizeDelta;
        rect.anchoredPosition = Vector2.zero;

        return img;
    }

    /// <summary>
    /// 繝励Ο繧ｰ繝ｩ繝荳翫〒1x1繝斐け繧ｻ繝ｫ縺ｮ逋ｽ濶ｲ繧ｹ繝励Λ繧､繝医ｒ蜍慕噪縺ｫ逕滓・縺励※霑斐☆縲・    /// </summary>
    private Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// 繧ｿ繧､繝繧｢繝・・譎ゅ↓蜻ｼ縺ｳ蜃ｺ縺吶さ繝ｼ繝ｫ繝舌ャ繧ｯ繧堤匳骭ｲ縺吶ｋ縲・    /// GameManager 縺ｮ Start() 縺ｧ SetOnTimeUp(GameOver) 縺ｮ繧医≧縺ｫ蜻ｼ縺ｶ縺薙→縲・    /// </summary>
    public void SetOnTimeUp(System.Action callback)
    {
        onTimeUp = callback;
    }

    /// <summary>
    /// 繧ｿ繧､繝槭・繧貞・譛溷､縺ｫ繝ｪ繧ｻ繝・ヨ縺励※險域ｸｬ繧帝幕蟋九☆繧九・    /// </summary>
    public void Initialize()
    {
        currentTime = timeLimit;
        isRunning   = true;

        UpdateTimerUI();
    }

    /// <summary>
    /// 繧ｿ繧､繝槭・縺ｮ險域ｸｬ繧貞●豁｢縺吶ｋ縲・    /// </summary>
    public void Stop()
    {
        isRunning = false;
    }

    /// <summary>
    /// 豈弱ヵ繝ｬ繝ｼ繝蜻ｼ縺ｰ繧後∵ｮ九ｊ譎る俣縺ｮ貂帛ｰ大・逅・√♀繧医・谿九ｊ譎る俣縺ｫ蠢懊§縺欟I・医ヰ繝ｼ縲∵棧邱壹√ユ繧ｭ繧ｹ繝茨ｼ峨・謠ｺ繧後い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧・峩譁ｰ繧定｡後≧縲・    /// </summary>
    private void Update()
    {
        // --- 髴・∴繧九い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・域凾髢薙′谿九ｊ蟆代↑縺上↑縺｣縺滓凾・・---
        if (timerContainer != null)
        {
            Vector2 randomOffset = Vector2.zero;

            if (isRunning && currentTime <= shakeThreshold)
            {
                // Random.insideUnitCircle 繧剃ｽｿ縺｣縺ｦ蜈・・菴咲ｽｮ縺ｮ蜻ｨ蝗ｲ繧偵Λ繝ｳ繝繝縺ｫ繝悶Ξ縺輔○繧・                randomOffset = Random.insideUnitCircle * shakeAmount;
            }

            // 蠎ｧ讓吶ｒ驕ｩ逕ｨ
            if (containerRectTransform != null)
            {
                containerRectTransform.anchoredPosition = initialContainerPosition + randomOffset;
            }
            else
            {
                timerContainer.localPosition = new Vector3(initialContainerPosition.x + randomOffset.x, initialContainerPosition.y + randomOffset.y, 0f);
            }
        }

        // --- 譫邱壹・髴・∴繧九い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・・遘剃ｻ･荳具ｼ・---
        if (borderContainerRect != null)
        {
            if (isRunning && currentTime <= 7f)
            {
                // 謠ｺ繧後ｒ謚代∴繧√↓隱ｿ謨ｴ・・遘剃ｻ･荳九〒蟆代＠縺縺大ｼｷ縺擾ｼ・                float currentShakeAmount = currentTime <= 3f ? shakeAmount * 0.8f : shakeAmount * 0.4f;
                Vector2 randomOffset = Random.insideUnitCircle * currentShakeAmount;
                borderContainerRect.anchoredPosition = initialBorderPosition + randomOffset;
            }
            else
            {
                borderContainerRect.anchoredPosition = initialBorderPosition;
            }
        }

        if (!isRunning) return;

        // 繧ｿ繧､繝槭・縺ｯ蟶ｸ譎よｸ帛ｰ代☆繧・        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;

            UpdateTimerUI();

            Debug.Log("Time Up!");

            // 繧ｿ繧､繝繧｢繝・・繧竪ameManager縺ｫ騾夂衍縺吶ｋ
            onTimeUp?.Invoke();
        }
        else
        {
            UpdateTimerUI();
        }
    }

    /// <summary>
    /// 繝壹リ繝ｫ繝・ぅ縺ｪ縺ｩ縺ｧ譎る俣繧呈ｸ帙ｉ縺・    /// </summary>
    public void ReduceTime(float amount)
    {
        if (!isRunning) return;
        
        currentTime -= amount;
        if (currentTime < 0f) currentTime = 0f;
        
        // 貂帙▲縺溽峩蠕後↓UI繧呈峩譁ｰ縺吶ｋ
        UpdateTimerUI();
    }

    /// <summary>
    /// 繧ｿ繧､繝槭・縺ｮImage・・ill Amount繝ｻ濶ｲ・峨→繝・く繧ｹ繝医ｒ譖ｴ譁ｰ縺吶ｋ縲・    /// </summary>
    private void UpdateTimerUI()
    {
        // 谿九ｊ譎る俣縺ｮ蜑ｲ蜷・(0.0 縲・1.0) 繧定ｨ育ｮ・        float timeRatio = currentTime / timeLimit;

        // Gradient繧剃ｽｿ縺｣縺ｦ縲∵ｮ九ｊ譎る俣縺ｮ蜑ｲ蜷医↓蠢懊§縺溯牡繧貞叙蠕励＠縺ｦ驕ｩ逕ｨ縺吶ｋ
        Color baseFrameColor = colorGradient.Evaluate(timeRatio);
        Color frameColor = baseFrameColor;

        // 譎る俣縺悟ｰ代↑縺・凾縺ｫ譫邱壹ｒ繝√き繝√き縺輔○繧区ｼ泌・・・遘剃ｻ･荳具ｼ・        float flash = 0f;
        if (isRunning && currentTime <= 7f)
        {
            float speedMultiplier = currentTime <= 3f ? 20f : 12f;
            flash = Mathf.Abs(Mathf.Sin(Time.time * speedMultiplier)); // 轤ｹ貊・            
            // 蜈・・濶ｲ縺ｨ襍､濶ｲ繧定｡後″譚･縺輔○縺ｦ繝√き繝√き縺輔○繧・            frameColor = Color.Lerp(baseFrameColor, Color.red, flash);
        }

        // UI譫邱壹ご繝ｼ繧ｸ縺ｮ譖ｴ譁ｰ
        if (borderImages != null && borderImages.Length == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                if (borderImages[i] != null)
                {
                    borderImages[i].color = frameColor;
                    
                    // 蜷・・繝ｼ繝繝ｼ縺ｯ蜈ｨ菴薙・25%蛻・ｒ諡・ｽ・(Top: 1.0~0.75, Right: 0.75~0.5, Bottom: 0.5~0.25, Left: 0.25~0)
                    float borderMin = (3 - i) * 0.25f;
                    float borderMax = (4 - i) * 0.25f;

                    if (timeRatio >= borderMax)
                    {
                        borderImages[i].fillAmount = 1f;
                    }
                    else if (timeRatio <= borderMin)
                    {
                        borderImages[i].fillAmount = 0f;
                    }
                    else
                    {
                        borderImages[i].fillAmount = (timeRatio - borderMin) / 0.25f;
                    }
                }
            }
        }

        // 2. 繝・く繧ｹ繝医・譖ｴ譁ｰ縺ｨ貍泌・
        string text;
        Color textColor = new Color(frameColor.r, frameColor.g, frameColor.b, 1f);
        float textScale = 1f;

        if (currentTime <= shakeThreshold)
        {
            // 谿九ｊ譎る俣縺悟ｰ代↑縺・凾縺ｯ繝溘Μ遘偵∪縺ｧ陦ｨ遉ｺ縺励※蛻・ｿｫ諢溘ｒ蜃ｺ縺・            text = currentTime.ToString("F2");
            // 蠢・∮縺ｮ鮠灘虚縺ｮ繧医≧縺ｫ繧ｹ繧ｱ繝ｼ繝ｫ繧呈ｳ｢謇薙◆縺帙ｋ
            textScale = Mathf.Lerp(1.0f, 1.4f, flash);
            // 繝・く繧ｹ繝郁牡繧りｵ､縺｣縺ｽ縺冗せ貊・＆縺帙ｋ
            textColor = Color.Lerp(Color.red, frameColor, flash);
        }
        else
        {
            text = Mathf.CeilToInt(currentTime).ToString();
        }

        if (timerText != null)    
        {
            timerText.text = text;
            timerText.color = textColor;
            timerText.transform.localScale = Vector3.one * textScale;
        }
        
        if (timerTextTMP != null) 
        {
            timerTextTMP.text = text;
            timerTextTMP.color = textColor;
            timerTextTMP.transform.localScale = Vector3.one * textScale;
        }
    }
}

