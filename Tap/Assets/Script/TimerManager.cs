using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// タイマーの計測とUI表示を担当するクラス。
/// 残り時間が減るにつれて、設定したグラデーション（Gradient）に従ってバーの色が滑らかに変化します。
///
/// 【Inspectorでの設定方法】
/// 1. timerImage: Imageの「Image Type」を「Filled」にし、「Fill Method」を「Horizontal」等に設定したUIオブジェクトをアサイン。
/// 2. colorGradient: タイマーの残り時間に応じた色の変化を設定します。（例：右端(1.0)を水色、左端(0.0)を赤色）
/// 3. timerTextTMP / timerText (任意): 数値でも表示したい場合はテキストをアサイン。
/// </summary>
public class TimerManager : MonoBehaviour
{
    // --- シングルトン（どこからでも簡単にアクセスできるようにする） ---
    public static TimerManager Instance { get; private set; }

    // --- 定数 ---
    private const string TIME_PREFIX = "Time: ";

    // --- タイマー基本設定 ---
    [Header("タイマー設定")]
    [Tooltip("ゲームの制限時間（秒）")]
    [SerializeField] private float timeLimit = 30.0f;

    // --- タイマーUI設定 ---
    [Header("タイマーUI (Fill Amount対応)")]
    [Tooltip("残り時間に応じて減っていくバーのImageコンポーネント")]
    [SerializeField] private Image timerImage;
    
    [Tooltip("残り時間に応じた色の変化。右端(1.0)が満タン時、左端(0.0)が時間切れ時の色になります")]
    [SerializeField] private Gradient colorGradient;

    // --- タイマーテキストUI (オプション) ---
    [Header("タイマーテキストUI (オプション)")]
    [SerializeField] private TMPro.TMP_Text timerTextTMP;
    [SerializeField] private Text     timerText;

    // --- 震えるアニメーション設定 ---
    [Header("アニメーション設定")]
    [Tooltip("震えさせたいUIのTransform（バーやテキストの親オブジェクトなど）")]
    [SerializeField] private Transform timerContainer;
    [Tooltip("残り何秒以下になったら震え始めるか")]
    [SerializeField] private float shakeThreshold = 5f;
    [Tooltip("震える激しさ（振幅）")]
    [SerializeField] private float shakeAmount = 3f;

    // --- 内部状態 ---
    private float currentTime;
    private bool  isRunning;
    private Vector2 initialContainerPosition;
    private RectTransform containerRectTransform;

    // タイムアップ時に呼ぶコールバック（GameManager から登録する）
    private System.Action onTimeUp;

    /// <summary>残り時間を外部から読み取り専用で公開する。</summary>
    public float CurrentTime => currentTime;

    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
        }

        // Paletteを使って色を統一
        colorGradient = Palette.GetTimerGradient();
    }

    private Image[] borderImages = new Image[4];
    private Sprite whiteSprite;
    private RectTransform borderContainerRect; // 枠線を揺らすための親コンテナ
    private Vector2 initialBorderPosition;

    private void Start()
    {
        // UIの初期位置をゲーム開始直後に保存しておく
        if (timerContainer != null)
        {
            containerRectTransform = timerContainer.GetComponent<RectTransform>();
            if (containerRectTransform != null)
            {
                initialContainerPosition = containerRectTransform.anchoredPosition;
            }
            else
            {
                // RectTransformがない場合（通常はないが念のため）
                initialContainerPosition = timerContainer.localPosition;
            }
        }

        GenerateFrameTimerUI();
    }

    private void GenerateFrameTimerUI()
    {
        // 以前のタイマーUIがあれば非表示にする
        if (timerImage != null)
        {
            timerImage.gameObject.SetActive(false);
        }

        GameObject canvasObj = new GameObject("FrameTimerCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // 最前面に表示して枠にする

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        // ボーダーをまとめるコンテナを作成（このコンテナを揺らす）
        GameObject borderContainerObj = new GameObject("BorderContainer");
        borderContainerRect = borderContainerObj.AddComponent<RectTransform>();
        borderContainerRect.SetParent(canvasObj.transform, false);
        borderContainerRect.anchorMin = Vector2.zero;
        borderContainerRect.anchorMax = Vector2.one;
        borderContainerRect.sizeDelta = Vector2.zero;
        borderContainerRect.anchoredPosition = Vector2.zero;
        initialBorderPosition = borderContainerRect.anchoredPosition;

        whiteSprite = CreateWhiteSprite();
        float thickness = 40f; // 目立つように太めの枠線にする

        // 枠の線（上、右、下、左の順）
        // 時計回りに減っていくように設定（残りが左上の角に向かって消えていく）
        borderImages[0] = CreateBorder(borderContainerObj.transform, "TopBorder", 
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

    private Image CreateBorder(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Image.FillMethod fillMethod, int fillOrigin)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        
        img.sprite = whiteSprite; // Fillを機能させるにはスプライトが必須
        img.type = Image.Type.Filled;
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

    private Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// タイムアップ時に呼び出すコールバックを登録する。
    /// GameManager の Start() で SetOnTimeUp(GameOver) のように呼ぶこと。
    /// </summary>
    public void SetOnTimeUp(System.Action callback)
    {
        onTimeUp = callback;
    }

    /// <summary>
    /// タイマーを初期値にリセットして計測を開始する。
    /// </summary>
    public void Initialize()
    {
        currentTime = timeLimit;
        isRunning   = true;

        UpdateTimerUI();
    }

    /// <summary>
    /// タイマーの計測を停止する。
    /// </summary>
    public void Stop()
    {
        isRunning = false;
    }

    private void Update()
    {
        // --- 震えるアニメーション（時間が残り少なくなった時） ---
        if (timerContainer != null)
        {
            Vector2 randomOffset = Vector2.zero;

            if (isRunning && currentTime <= shakeThreshold)
            {
                // Random.insideUnitCircle を使って元の位置の周囲をランダムにブレさせる
                randomOffset = Random.insideUnitCircle * shakeAmount;
            }

            // 座標を適用
            if (containerRectTransform != null)
            {
                containerRectTransform.anchoredPosition = initialContainerPosition + randomOffset;
            }
            else
            {
                timerContainer.localPosition = new Vector3(initialContainerPosition.x + randomOffset.x, initialContainerPosition.y + randomOffset.y, 0f);
            }
        }

        // --- 枠線の震えるアニメーション（7秒以下） ---
        if (borderContainerRect != null)
        {
            if (isRunning && currentTime <= 7f)
            {
                // 揺れを抑えめに調整（3秒以下で少しだけ強く）
                float currentShakeAmount = currentTime <= 3f ? shakeAmount * 0.8f : shakeAmount * 0.4f;
                Vector2 randomOffset = Random.insideUnitCircle * currentShakeAmount;
                borderContainerRect.anchoredPosition = initialBorderPosition + randomOffset;
            }
            else
            {
                borderContainerRect.anchoredPosition = initialBorderPosition;
            }
        }

        if (!isRunning) return;

        // タイマーは常時減少する
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;

            UpdateTimerUI();

            Debug.Log("Time Up!");

            // タイムアップをGameManagerに通知する
            onTimeUp?.Invoke();
        }
        else
        {
            UpdateTimerUI();
        }
    }

    /// <summary>
    /// ペナルティなどで時間を減らす
    /// </summary>
    public void ReduceTime(float amount)
    {
        if (!isRunning) return;
        
        currentTime -= amount;
        if (currentTime < 0f) currentTime = 0f;
        
        // 減った直後にUIを更新する
        UpdateTimerUI();
    }

    /// <summary>
    /// タイマーのImage（Fill Amount・色）とテキストを更新する。
    /// </summary>
    private void UpdateTimerUI()
    {
        // 残り時間の割合 (0.0 〜 1.0) を計算
        float timeRatio = currentTime / timeLimit;

        // Gradientを使って、残り時間の割合に応じた色を取得して適用する
        Color baseFrameColor = colorGradient.Evaluate(timeRatio);
        Color frameColor = baseFrameColor;

        // 時間が少ない時に枠線をチカチカさせる演出（7秒以下）
        float flash = 0f;
        if (isRunning && currentTime <= 7f)
        {
            float speedMultiplier = currentTime <= 3f ? 20f : 12f;
            flash = Mathf.Abs(Mathf.Sin(Time.time * speedMultiplier)); // 点滅
            
            // 元の色と赤色を行き来させてチカチカさせる
            frameColor = Color.Lerp(baseFrameColor, Color.red, flash);
        }

        // UI枠線ゲージの更新
        if (borderImages != null && borderImages.Length == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                if (borderImages[i] != null)
                {
                    borderImages[i].color = frameColor;
                    
                    // 各ボーダーは全体の25%分を担当 (Top: 1.0~0.75, Right: 0.75~0.5, Bottom: 0.5~0.25, Left: 0.25~0)
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

        // 2. テキストの更新と演出
        string text;
        Color textColor = new Color(frameColor.r, frameColor.g, frameColor.b, 1f);
        float textScale = 1f;

        if (currentTime <= shakeThreshold)
        {
            // 残り時間が少ない時はミリ秒まで表示して切迫感を出す
            text = currentTime.ToString("F2");
            // 心臓の鼓動のようにスケールを波打たせる
            textScale = Mathf.Lerp(1.0f, 1.4f, flash);
            // テキスト色も赤っぽく点滅させる
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

