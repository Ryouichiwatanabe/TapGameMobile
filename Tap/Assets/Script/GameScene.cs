using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 繧ｲ繝ｼ繝蜈ｨ菴薙・騾ｲ陦檎ｮ｡逅・ｼ磯幕蟋九・邨ゆｺ・・繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ・峨ｒ諡・ｽ薙☆繧句昇莉､蝪斐け繝ｩ繧ｹ縲・/// 繧ｹ繧ｳ繧｢蜃ｦ逅・・ ScoreManager 縺ｸ縲√ち繧､繝槭・蜃ｦ逅・・ TimerManager 縺ｸ蟋碑ｭｲ縺吶ｋ縲・///
/// 縲蝕nspector縺ｧ縺ｮ險ｭ螳壽婿豕輔・/// (1) ScoreManager 繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ繧｢繧ｿ繝・メ縺励◆ GameObject 繧・scoreManager 繝輔ぅ繝ｼ繝ｫ繝峨↓繧｢繧ｵ繧､繝ｳ縺吶ｋ縲・/// (2) TimerManager 繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ繧｢繧ｿ繝・メ縺励◆ GameObject 繧・timerManager 繝輔ぅ繝ｼ繝ｫ繝峨↓繧｢繧ｵ繧､繝ｳ縺吶ｋ縲・///     窶ｻ 蜷御ｸ GameObject 縺ｫ縺ｾ縺ｨ繧√※繧｢繧ｿ繝・メ縺励※繧ゅ∝挨縲・・ GameObject 縺ｫ蛻・￠縺ｦ繧ゅ←縺｡繧峨〒繧ょ庄縲・/// (3) circlePrefab 縺ｫ逕滓・縺吶ｋ蜀・・繝励Ξ繝上ヶ繧偵い繧ｵ繧､繝ｳ縺吶ｋ縲・/// (4) gameOverUI 縺ｫ繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・譎ゅ↓陦ｨ遉ｺ縺吶ｋ繝代ロ繝ｫ繧偵い繧ｵ繧､繝ｳ縺吶ｋ縲・/// (5) fadeImage繝ｻfadeDuration繝ｻnextSceneName 縺ｧ繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ縺ｮ繝輔ぉ繝ｼ繝芽ｨｭ螳壹ｒ陦後≧縲・/// </summary>
public class GameScene : MonoBehaviour
{
    // --- 蜀・・逕滓・險ｭ螳・---
    [Header("蜀・・逕滓・險ｭ螳・)]
    [SerializeField] private GameObject circlePrefab;

    // Circle繝励Ξ繝上ヶ縺ｮ蜊雁ｾ・ｼ育判髱｢蜀・↓蜿弱∪繧九ｈ縺・％縺ｮ蛻・□縺大・蛛ｴ縺ｫ蛻ｶ髯舌☆繧具ｼ・    // Circle繝励Ξ繝上ヶ縺ｮ螳滄圀縺ｮ螟ｧ縺阪＆縺ｫ蜷医ｏ縺帙※Inspector縺ｧ隱ｿ謨ｴ縺吶ｋ縺薙→
    [SerializeField] private float circleRadius = 0.5f;

    [Header("逕ｻ髱｢遶ｯ縺九ｉ縺ｮ菴咏區・・I縺ｨ陲ｫ繧峨↑縺・ｈ縺・↓隱ｿ謨ｴ・・)]
    [SerializeField] private float paddingBottom = 2.0f; // 荳矩Κ縺ｮUI縺ｪ縺ｩ繧帝∩縺代ｋ縺溘ａ縺ｮ菴咏區
    [SerializeField] private float paddingTop    = 0.5f;
    [SerializeField] private float paddingLeft   = 0.5f;
    [SerializeField] private float paddingRight  = 0.5f;

    // 繧ｹ繝昴・繝ｳ蜿ｯ閭ｽ縺ｪ逕ｻ髱｢蜀・・遽・峇・・tart()縺ｧCamera縺九ｉ閾ｪ蜍戊ｨ育ｮ励＆繧後ｋ・・    private float spawnMinX, spawnMaxX;
    private float spawnMinY, spawnMaxY;

    // 谺｡縺ｮCircle蜃ｺ迴ｾ菴咲ｽｮ縺ｫ陦ｨ遉ｺ縺吶ｋ阮・＞霈ｪ繧ｨ繝輔ぉ繧ｯ繝医・繝励Ξ繝上ヶ
    // SpawnHintEffect 繧ｹ繧ｯ繝ｪ繝励ヨ繧偵い繧ｿ繝・メ縺励◆遨ｺ縺ｮGameObject繧偵・繝ｬ繝上ヶ蛹悶＠縺ｦ繧｢繧ｵ繧､繝ｳ縺吶ｋ
    [SerializeField] private GameObject spawnHintPrefab;

    // 豸医∴縺溷・ 竊・谺｡縺ｮ蜃ｺ迴ｾ菴咲ｽｮ縺ｸ蜷代°縺・ｷ壹お繝輔ぉ繧ｯ繝医・繝励Ξ繝上ヶ
    // ConnectLineEffect 繧ｹ繧ｯ繝ｪ繝励ヨ繧偵い繧ｿ繝・メ縺励◆遨ｺ縺ｮGameObject繧偵・繝ｬ繝上ヶ蛹悶＠縺ｦ繧｢繧ｵ繧､繝ｳ縺吶ｋ
    [SerializeField] private GameObject spawnConnectLinePrefab;

    [Header("繧ｨ繝輔ぉ繧ｯ繝郁ｨｭ螳・)]
    [Tooltip("繧ｹ繧ｳ繧｢縺御ｸ翫′縺｣縺滓凾縺ｫ逕ｻ髱｢荳｡遶ｯ縺九ｉ蜃ｺ繧九後・繧翫・繧翫阪お繝輔ぉ繧ｯ繝・)]
    [SerializeField] private GameObject sideZapEffectPrefab;

    // --- 繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・UI ---
    // Inspector縺ｧ繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・譎ゅ↓陦ｨ遉ｺ縺吶ｋ繝代ロ繝ｫ繧偵い繧ｵ繧､繝ｳ縺吶ｋ
    [Header("繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・UI")]
    [SerializeField] private GameObject gameOverUI;

    // --- 繝輔ぉ繝ｼ繝芽ｨｭ螳・---
    [Header("繝輔ぉ繝ｼ繝芽ｨｭ螳・)]
    [SerializeField] private Image  fadeImage;
  //  [SerializeField] private float  fadeDuration  = 1f;
    [SerializeField] private string nextSceneName = "Result";

    // --- 蜷・Manager 縺ｸ縺ｮ蜿ら・ ---
    // Inspector縺ｧ縺昴ｌ縺槭ｌ縺ｮManager繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ繧｢繧ｵ繧､繝ｳ縺吶ｋ
    [Header("蜷・Manager・・nspector縺ｧ繧｢繧ｵ繧､繝ｳ・・)]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimerManager timerManager;

    // --- 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳUI ---
    [Header("繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳUI")]
    [Tooltip("繧ｲ繝ｼ繝髢句ｧ句燕縺ｮ繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ繧定｡ｨ遉ｺ縺吶ｋ TextMeshPro")]
    [SerializeField] private TMPro.TMP_Text countdownTextTMP;
    [Tooltip("繧ｲ繝ｼ繝髢句ｧ句燕縺ｮ繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ繧定｡ｨ遉ｺ縺吶ｋ 蠕捺擂縺ｮ Text")]
    [SerializeField] private Text countdownText;

    // --- 蜀・Κ迥ｶ諷具ｼ医せ繝・・繝医・繧ｷ繝ｳ・・---
    public enum GameState
    {
        Countdown,
        Playing,
        GameOver,
        Transitioning
    }

    /// <summary>迴ｾ蝨ｨ縺ｮ繧ｲ繝ｼ繝縺ｮ迥ｶ諷・/summary>
    public GameState CurrentState { get; private set; }

    /// <summary>螟夜Κ繧ｯ繝ｩ繧ｹ・・ircleController遲会ｼ峨°繧峨・蠕梧婿莠呈鋤逕ｨ繝励Ο繝代ユ繧｣</summary>
    public bool isGameActive => CurrentState == GameState.Playing;

    /// <summary>迴ｾ蝨ｨ縺ｮ繧ｹ繧ｳ繧｢繧貞叙蠕励☆繧・/summary>
    public int CurrentScore => scoreManager != null ? scoreManager.Score : 0;

    [Header("BGM險ｭ螳・)]
    [Tooltip("GameScene縺ｧ豬√☆BGM・域悴險ｭ螳壹・蝣ｴ蜷医・閾ｪ蜍輔〒GameBGM繧定ｪｭ縺ｿ霎ｼ縺ｿ縺ｾ縺呻ｼ・)]
    [SerializeField] private AudioClip bgmClip;


    /// <summary>
    /// 繧ｲ繝ｼ繝髢句ｧ区凾縺ｮ蛻晄悄蛹門・逅・りレ譎ｯ險ｭ螳壹。GM貅門ｙ縲ゞI蛻晄悄蛹悶√ち繧､繝槭・繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ逋ｻ骭ｲ縺ｪ縺ｩ繧定｡後≧縲・    /// </summary>
    void Start()
    {
        // --- 閭梧勹逕ｻ蜒上・險ｭ螳・---
        BackgroundHelper.SetupBackground("bg_game");

        // BGM縺ｯ繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ螳御ｺ・ｾ鯉ｼ・O!縺ｮ繧ｿ繧､繝溘Φ繧ｰ・峨↓蜀咲函縺吶ｋ縺溘ａ縲√％縺薙〒縺ｯ貅門ｙ縺ｮ縺ｿ
        if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Audio/GameBGM");

        // 蛻晄悄迥ｶ諷九・繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ
        CurrentState = GameState.Countdown;

        // 繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・UI繧帝撼陦ｨ遉ｺ縺ｫ縺励※蛻晄悄蛹・        if (gameOverUI != null) gameOverUI.SetActive(false);

        // 繝輔ぉ繝ｼ繝峨う繝ｳ貅門ｙ・育判髱｢繧帝乗・縺ｪ鮟偵°繧牙ｧ九ａ繧具ｼ・        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0, 0, 0, 0);
        }

        // TimerManager 縺ｮ繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ縺縺大・縺ｫ逋ｻ骭ｲ縺励※縺翫￥・郁ｨ域ｸｬ髢句ｧ九・蠕鯉ｼ・        if (timerManager != null)
        {
            timerManager.SetOnTimeUp(GameOver);
        }

        // 逕ｻ髱｢縺ｮ繝ｯ繝ｼ繝ｫ繝牙ｺｧ讓咏ｯ・峇繧定ｨ育ｮ励☆繧具ｼ亥・縺檎判髱｢蜀・↓蜿弱∪繧九ｈ縺・↓縺吶ｋ・・        CalculateSpawnBounds();

        // 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ蜃ｦ逅・ｒ繧ｹ繧ｿ繝ｼ繝・        StartCoroutine(CountdownRoutine());
    }

    /// <summary>
    /// 3, 2, 1, START! 縺ｨ繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ縺励※縺九ｉ繧ｲ繝ｼ繝繧呈悽譬ｼ逧・↓髢句ｧ九☆繧九さ繝ｫ繝ｼ繝√Φ
    /// </summary>
    private IEnumerator CountdownRoutine()
    {
        string[] counts = {  "Ready" }; // 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ繧貞ｰ代＠繝ｪ繝・メ縺ｫ
        
        foreach (string c in counts)
        {
            if (countdownTextTMP != null) countdownTextTMP.text = c;
            if (countdownText != null)    countdownText.text = c;
            // 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ髻ｳ縲後ヴ繝・・            if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownTick();

            // 繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ: 螟ｧ縺阪￥縺励※縺九ｉ蜈・・繧ｵ繧､繧ｺ縺ｸ
            yield return StartCoroutine(AnimateText(1f));
        }

        // 繧ｹ繧ｿ繝ｼ繝郁｡ｨ遉ｺ
        if (countdownTextTMP != null) countdownTextTMP.text = " GO!";

        // 繧ｲ繝ｼ繝髢句ｧ矩浹縲後・繝ｼ繝ｳ・√・        if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownGo();

        // --- 縺薙％縺ｧBGM繧貞・逕滄幕蟋・---
        if (AudioManager.Instance != null && bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(bgmClip, 0.4f);
        }

        yield return StartCoroutine(AnimateText(0.5f, 1.5f));

        // 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ繝・く繧ｹ繝医ｒ髱櫁｡ｨ遉ｺ縺ｫ縺吶ｋ
        if (countdownTextTMP != null) countdownTextTMP.gameObject.SetActive(false);


        // --- 縺薙％縺九ｉ譛ｬ譬ｼ逧・↓繧ｲ繝ｼ繝髢句ｧ・---
        
        // 繧ｹ繧ｳ繧｢縺ｨ繧ｿ繧､繝槭・縺ｮ蛻晄悄蛹厄ｼ・ｨ域ｸｬ髢句ｧ・        if (scoreManager != null) scoreManager.Initialize();
        if (timerManager != null) timerManager.Initialize();



        // 繝励Ξ繧､繝､繝ｼ縺ｮ謫堺ｽ懊ｒ蜿励￠莉倥￠繧狗憾諷具ｼ・laying・峨↓螟画峩
        CurrentState = GameState.Playing;

        // 譛蛻昴・蜀・ｒ逕滓・縺吶ｋ
        SpawnNextCircle();
    }

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ逕ｨ繝・く繧ｹ繝医ｒ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・域僑螟ｧ繝ｻ邵ｮ蟆擾ｼ峨＆縺帙ｋ繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator AnimateText(float duration, float maxScale = 1.2f)
    {
        float elapsed = 0f;
        Transform targetTransform = null;
        if (countdownTextTMP != null) targetTransform = countdownTextTMP.transform;
        else if (countdownText != null) targetTransform = countdownText.transform;

        if (targetTransform == null)
        {
            yield return new WaitForSeconds(duration);
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 0 -> 1 縺ｮ髢薙〒繧ｵ繧､繝ｳ豕｢繧剃ｽｿ縺｣縺ｦ繧ｹ繧ｱ繝ｼ繝ｫ繧貞､ｧ縺阪￥縺励※謌ｻ縺・            float scale = 1f + Mathf.Sin(t * Mathf.PI) * (maxScale - 1f);
            targetTransform.localScale = Vector3.one * scale;
            yield return null;
        }
        targetTransform.localScale = Vector3.one;
    }


    /// <summary>
    /// Camera 縺ｮ繝ｯ繝ｼ繝ｫ繝牙ｺｧ讓吶°繧峨∝・縺檎判髱｢蜀・↓螳悟・縺ｫ蜿弱∪繧九せ繝昴・繝ｳ遽・峇繧定ｨ育ｮ励＠縺ｦ繧ｭ繝｣繝・す繝･縺吶ｋ縲・    /// 隗｣蜒丞ｺｦ繝ｻ繧｢繧ｹ繝壹け繝域ｯ斐′逡ｰ縺ｪ繧九ョ繝舌う繧ｹ縺ｧ繧り・蜍慕噪縺ｫ豁｣縺励＞遽・峇縺瑚ｨｭ螳壹＆繧後ｋ縲・    /// Start() 縺九ｉ蜻ｼ縺ｶ縺薙→縲・    /// </summary>
    private void CalculateSpawnBounds()
    {
        Camera cam = Camera.main;

        // 繧ｫ繝｡繝ｩ縺九ｉ蜀・′蟄伜惠縺吶ｋ繝ｯ繝ｼ繝ｫ繝瓜=0 蟷ｳ髱｢縺ｾ縺ｧ縺ｮ霍晞屬繧呈ｱゅａ繧・        // 2D繧ｲ繝ｼ繝縺ｧ縺ｯ Camera.main 縺ｯ騾壼ｸｸ Z=-10 縺ｫ驟咲ｽｮ縺輔ｌ縺ｦ縺・ｋ縺溘ａ霍晞屬縺ｯ 10
        float dist = Mathf.Abs(cam.transform.position.z);
        if (dist < 0.01f) dist = 10f; // 繧ｫ繝｡繝ｩ縺兄=0縺ｫ縺ゅｋ蝣ｴ蜷医・繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ

        // 繝薙Η繝ｼ繝昴・繝亥ｺｧ讓呻ｼ・0,0) = 逕ｻ髱｢蟾ｦ荳九・1,1) = 逕ｻ髱｢蜿ｳ荳・繧偵Ρ繝ｼ繝ｫ繝牙ｺｧ讓吶↓螟画鋤
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0f, dist));
        Vector3 topRight   = cam.ViewportToWorldPoint(new Vector3(1f, 1f, dist));

        // Circle繝励Ξ繝上ヶ縺ｮ螳滄圀縺ｮ蜊雁ｾ・ｒ繧ｳ繝ｳ繝昴・繝阪Φ繝医°繧芽・蜍募叙蠕・        float halfSize = GetCircleHalfSize();

        // 蜊雁ｾ・+ Inspector縺ｧ險ｭ螳壹＠縺溷推譁ｹ蜷代・繝代ョ繧｣繝ｳ繧ｰ蛻・□縺大・蛛ｴ縺ｫ蛻ｶ髯舌☆繧・        spawnMinX = bottomLeft.x + halfSize + paddingLeft;
        spawnMaxX = topRight.x   - halfSize - paddingRight;
        spawnMinY = bottomLeft.y + halfSize + paddingBottom;
        spawnMaxY = topRight.y   - halfSize - paddingTop;

      //  Debug.Log($"[GameManager] Circle蜊雁ｾ・{halfSize:F2} | SpawnBounds: X({spawnMinX:F2}縲悳spawnMaxX:F2})  Y({spawnMinY:F2}縲悳spawnMaxY:F2})");
    }

    /// <summary>
    /// circlePrefab 縺ｮ繧ｳ繝ｳ繝昴・繝阪Φ繝医°繧峨せ繧ｱ繝ｼ繝ｫ霎ｼ縺ｿ縺ｮ蜊雁ｾ・ｒ閾ｪ蜍募叙蠕励☆繧九・    /// 蜿門ｾ怜━蜈磯・ｽ・ CircleCollider2D 竊・SpriteRenderer 竊・Inspector謇句虚蛟､(circleRadius)
    /// </summary>
    private float GetCircleHalfSize()
    {
        if (circlePrefab == null) return circleRadius;

        // 竭 CircleCollider2D 縺後≠繧句ｴ蜷茨ｼ域怙繧よｭ｣遒ｺ・・        // radius ﾃ・scale.x 縺ｧ繝ｯ繝ｼ繝ｫ繝峨せ繧ｱ繝ｼ繝ｫ縺ｮ蜊雁ｾ・′蠕励ｉ繧後ｋ
        CircleCollider2D col2D = circlePrefab.GetComponent<CircleCollider2D>();
        if (col2D != null)
            return col2D.radius * circlePrefab.transform.localScale.x;

        // 竭｡ SpriteRenderer 縺後≠繧句ｴ蜷・        // sprite 縺ｮ bounds 縺九ｉ蜊雁ｾ・ｒ謗ｨ螳壹☆繧・        SpriteRenderer sr = circlePrefab.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            return sr.sprite.bounds.extents.x * circlePrefab.transform.localScale.x;

        // 竭｢ 荳願ｨ倥←縺｡繧峨ｂ縺ｪ縺代ｌ縺ｰ Inspector 縺ｮ circleRadius 繧偵ヵ繧ｩ繝ｼ繝ｫ繝舌ャ繧ｯ縺ｨ縺励※菴ｿ逕ｨ
       // Debug.LogWarning("[GameManager] Circle 縺ｮ蜊雁ｾ・ｒ閾ｪ蜍募叙蠕励〒縺阪∪縺帙ｓ縺ｧ縺励◆縲ＤircleRadius 縺ｮ蛟､繧剃ｽｿ逕ｨ縺励∪縺吶・);
        return circleRadius;
    }

    /// <summary>
    /// 豈弱ヵ繝ｬ繝ｼ繝縺ｮ蜃ｦ逅・ｼ育樟蝨ｨ縺ｯ閾ｪ蜍輔す繝ｼ繝ｳ驕ｷ遘ｻ縺ｮ縺溘ａ迚ｹ蛻･縺ｪ蜃ｦ逅・・縺ｪ縺暦ｼ峨・    /// </summary>
    void Update()
    {
        // 閾ｪ蜍輔〒繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ縺吶ｋ繧医≧縺ｫ縺励◆縺溘ａ縲ゞpdate縺ｧ縺ｮ繧ｿ繝・・蠕・■縺ｯ蜑企勁
    }

    /// <summary>
    /// 谺｡縺ｮ蜀・ｒ繝ｩ繝ｳ繝繝縺ｪ菴咲ｽｮ縺ｫ逕滓・縺吶ｋ縲・    /// 蠑墓焚縺ｪ縺・ Start()縺九ｉ蜻ｼ縺ｰ繧後ｋ・域怙蛻昴・蜀・↑縺ｮ縺ｧ邱壹・荳崎ｦ・ｼ峨・    /// fromPosition 謖√■: CircleController縺九ｉ繧ｿ繝・・譎ゅ↓蜻ｼ縺ｰ繧後ｋ・育ｷ壹お繝輔ぉ繧ｯ繝医≠繧奇ｼ峨・    /// </summary>
    /// <param name="fromPosition">蜑阪・蜀・・蠎ｧ讓呻ｼ医↑縺代ｌ縺ｰ邱壹お繝輔ぉ繧ｯ繝医・逕滓・縺励↑縺・ｼ・/param>
    public Vector3 SpawnNextCircle(Vector3? fromPosition = null)
    {
        if (!isGameActive) return Vector3.zero;

        // 逕ｻ髱｢蜀・・遽・峇蜀・〒繝ｩ繝ｳ繝繝縺ｪ蠎ｧ讓吶ｒ蜀ｳ螳壹☆繧・        // spawnMin/Max 縺ｯ CalculateSpawnBounds() 縺ｧ繧ｫ繝｡繝ｩ縺ｮ繝ｯ繝ｼ繝ｫ繝牙ｺｧ讓吶°繧芽ｨ育ｮ玲ｸ医∩
        float randomX = Random.Range(spawnMinX, spawnMaxX);
        float randomY = Random.Range(spawnMinY, spawnMaxY);
        Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

        // --- 邱壹お繝輔ぉ繧ｯ繝・---
        // fromPosition 縺梧ｸ｡縺輔ｌ縺滓凾縺ｮ縺ｿ螳溯｡鯉ｼ域怙蛻昴・蜀・函謌先凾縺ｯ繧ｹ繧ｭ繝・・・・        if (fromPosition.HasValue && spawnConnectLinePrefab != null)
        {
            // 邱壹お繝輔ぉ繧ｯ繝医・繝励Ξ繝上ヶ繧堤函謌舌＠縲￣lay()縺ｧ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ髢句ｧ・            // 繧ｪ繝悶ず繧ｧ繧ｯ繝医・ ConnectLineEffect 蜀・Κ縺ｧ閾ｪ蟾ｱ遐ｴ譽・＆繧後ｋ縺ｮ縺ｧ螟悶°繧臥ｮ｡逅・ｸ崎ｦ・            GameObject lineObj = Instantiate(spawnConnectLinePrefab, Vector3.zero, Quaternion.identity);
            ConnectLineEffect lineEffect = lineObj.GetComponent<ConnectLineEffect>();
            if (lineEffect != null)
                lineEffect.Play(fromPosition.Value, spawnPos);
        }

        // --- 霈ｪ繧ｨ繝輔ぉ繧ｯ繝茨ｼ亥・迴ｾ菴咲ｽｮ縺ｮ逶ｮ蜊ｰ・・---
        // spawnHintPrefab 縺ｯ Inspector 縺ｧ繧｢繧ｵ繧､繝ｳ縺励※縺翫￥・域悴險ｭ螳壹↑繧我ｽ輔ｂ縺励↑縺・ｼ・        if (spawnHintPrefab != null)
            Instantiate(spawnHintPrefab, spawnPos, Quaternion.identity);

        // --- Circle 譛ｬ菴薙・逕滓・ ---
        GameObject circleObj = Instantiate(circlePrefab, spawnPos, Quaternion.identity);
        
        // 1. 蜀・・隕九◆逶ｮ繧・.3蛟阪↓螟ｧ縺阪￥縺吶ｋ
        circleObj.transform.localScale *= 1.3f;
        
        // 2. 繧ｿ繝・・蛻､螳夲ｼ亥ｽ薙◆繧雁愛螳夲ｼ峨ｒ隕九°縺代ｈ繧翫ｂ縺輔ｉ縺ｫ蟆代＠螟ｧ縺阪￥縺励※謚ｼ縺励ｄ縺吶￥縺吶ｋ・・20%・・        CircleCollider2D col = circleObj.GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.radius *= 1.2f;
        }

        return spawnPos;
    }

    /// <summary>
    /// 繧ｹ繧ｳ繧｢繧貞刈邂励☆繧九・    /// CircleController 縺九ｉ繧ｿ繝・・譎ゅ↓蜻ｼ縺ｰ繧後∝・驛ｨ縺ｧ ScoreManager 縺ｫ蟋碑ｭｲ縺吶ｋ縲・    /// </summary>
    /// <param name="points">蜉邂励☆繧九・繧､繝ｳ繝域焚</param>
    public int AddScore(int points)
    {
        if (!isGameActive) return 0;

        int added = 0;
        // ScoreManager 縺ｫ螳滄圀縺ｮ蜉邂怜・逅・ｒ莉ｻ縺帙ｋ
        if (scoreManager != null) added = scoreManager.AddScore(points);

        return added;
    }


    /// <summary>
    /// 繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・蜃ｦ逅・５imerManager 縺ｮ繧ｿ繧､繝繧｢繝・・繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ縺九ｉ蜻ｼ縺ｰ繧後ｋ縲・    /// </summary>
    private void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.GameOver;

        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlayMissSound();
            // 髻ｳ縺梧･縺ｫ豁｢縺ｾ繧九→荳崎・辟ｶ縺ｪ縺ｮ縺ｧ縲・.8遘偵°縺代※貊代ｉ縺九↓繝輔ぉ繝ｼ繝峨い繧ｦ繝医☆繧・            AudioManager.Instance.FadeOutBGM(0.5f);
        }

        // 繧ｿ繧､繝槭・繧貞●豁｢縺吶ｋ・亥ｿｵ縺ｮ縺溘ａ莠碁㍾蛛懈ｭ｢繧帝亟縺撰ｼ・        if (timerManager != null) timerManager.Stop();

        // 逕ｻ髱｢荳翫↓縺ゅｋCircle繧堤峩縺｡縺ｫ髱櫁｡ｨ遉ｺ・亥炎髯､・峨☆繧・        CircleController[] circles = Object.FindObjectsOfType<CircleController>();
        foreach(var c in circles)
        {
            if (c != null && c.gameObject != null)
            {
                c.gameObject.SetActive(false);
            }
        }

        // 繧ｹ繧ｳ繧｢繧・PlayerPrefs 縺ｫ菫晏ｭ倥＠縲∵怙邨ゅせ繧ｳ繧｢UI繧定｡ｨ遉ｺ縺吶ｋ
        if (scoreManager != null) scoreManager.SaveAndShowFinalScore();

        // 譎る俣縺ｮ豬√ｌ繧貞・縺ｫ謌ｻ縺呻ｼ医せ繝ｭ繝ｼ繝｢繝ｼ繧ｷ繝ｧ繝ｳ縺ｪ縺ｩ縺悟・縺｣縺ｦ縺・◆蝣ｴ蜷医∈縺ｮ蟇ｾ遲厄ｼ・        Time.timeScale = 1f;

        // 縺吶＄縺ｫResultScene縺ｸ遘ｻ陦後☆繧具ｼ医ヵ繝ｪ繝ｼ繧ｺ諢溘ｒ螳悟・縺ｫ縺ｪ縺上☆・・        StartCoroutine(FastTransitionToResult());
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・蠕後√ヵ繧ｧ繝ｼ繝峨い繧ｦ繝域ｼ泌・繧呈検繧薙〒騾溘ｄ縺九↓繝ｪ繧ｶ繝ｫ繝育判髱｢・・esultScene・峨∈驕ｷ遘ｻ縺吶ｋ繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator FastTransitionToResult()
    {
        // TimeUp 縺ｮUI繧剃ｸ迸ｬ縺縺大・縺呻ｼ井ｸ崎ｦ√↑蝣ｴ蜷医・蜑企勁蜿ｯ閭ｽ縺ｧ縺吶′縲∽ｸ蠢懆｡ｨ遉ｺ縺縺代＠縺ｾ縺呻ｼ・        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            gameOverUI.transform.localScale = Vector3.one;
            CanvasGroup cg = gameOverUI.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }

        CurrentState = GameState.Transitioning;

        // 0.5遘偵・繝輔ぉ繝ｼ繝峨い繧ｦ繝医〒繧ｷ繝ｼ繝ｳ繧貞・繧頑崛縺医ｋ
        float fastFadeDuration = 0.8f;
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fastFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(elapsed / fastFadeDuration));
                yield return null;
            }
        }

        if (SceneManager.Instance != null)
            SceneManager.Instance.LoadScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}