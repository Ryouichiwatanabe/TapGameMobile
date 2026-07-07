using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// スコアに応じたランクごとの演出設定をまとめる構造体
/// </summary>
[System.Serializable]
/// <summary>

public class ScoreRank
{
    [Tooltip("このランクになるために必要な最低スコア")]
    public int minScore = 0;
    
    [Tooltip("このランクの時のスコアテキストの色")]
    public Color scoreColor = Color.white; // デフォルトを白（不透明）にして透明になるのを防ぐ
    
    [Tooltip("テキストの震え幅（0なら震えない）")]
    public float shakeAmount = 0f;
    
    [Tooltip("テキストの震えスピード")]
    public float shakeSpeed = 10f;
    
    [Tooltip("このランクで出現させるパーティクルエフェクト（任意）")]
    public GameObject effectPrefab;
}

/// <summary>
/// リザルト画面（ResultScene）の制御を行うスクリプト。
/// スコアに応じて色、アニメーション、エフェクトを豪華にする機能を追加しています。
/// </summary>
public class ResultScene : MonoBehaviour
{
    // --- 定数 ---
    private const string LAST_SCORE_KEY = "LastScore";
    private const string NEXT_SCENE     = "Title";
    private const string TAP_TEXT       = "Tap to Title";

    // --- UI設定 ---
    [Header("UI")]
    [Tooltip("スコアを表示する TextMeshPro (ある場合)")]
    [SerializeField] private TMP_Text resultScoreTextTMP;
    [Tooltip("スコアを表示する 従来の Text (ある場合)")]
    [SerializeField] private Text     resultScoreText;
    
    [Tooltip("「Tap to Title」を表示する TextMeshPro (ある場合)")]
    [SerializeField] private TMP_Text tapToTitleTextTMP;
    [Tooltip("「Tap to Title」を表示する 従来の Text (ある場合)")]
    [SerializeField] private Text     tapToTitleText;

    // --- 演出設定 ---
    [Header("ランク別の演出設定（スコアが高い順に並べなくても自動ソートされます）")]
    [SerializeField] private List<ScoreRank> scoreRanks = new List<ScoreRank>();

    // --- フェード設定 ---
    [Header("フェード設定")]
    [SerializeField] private Image  fadeImage;
    [SerializeField] private float  fadeDuration  = 1f;
    [SerializeField] private string nextSceneName = NEXT_SCENE;

    // --- BGM設定 ---
    [Header("BGM設定")]
    [Tooltip("リザルト画面で流すBGM（未設定の場合は自動でResultBGMを読み込みます）")]
    [SerializeField] private AudioClip bgmClip;

    // 内部変数
    private bool isTransitioning;
    private int currentScore;
    private ScoreRank currentRank;
    private Vector2 initialScorePosition;
    private RectTransform scoreRectTransform;

    private void Start()
    {
        // --- 背景画像の設定 ---
        BackgroundHelper.SetupBackground("bg_result");

        // --- BGMの再生 ---
        if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Audio/ResultBGM");
        if (AudioManager.Instance != null && bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(bgmClip, 0.4f); // 音量は0.4f（少し控えめ）
        }

        // フェード用の画像を最初に透明にしておく
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
        }

        // --- スコアの読み込み ---
        currentScore = PlayerPrefs.GetInt(LAST_SCORE_KEY, 0);
        
        // TMPか通常のTextのどちらかに参照をセットアップし、初期表示を"0"にしておく
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

        // タップを促すテキストを表示する（最初は透明にしておく）
        if (tapToTitleTextTMP != null) 
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

        // --- ランク別演出の適用 ---
        ApplyRankEffects();

        // --- アニメーションの開始 ---
        StartCoroutine(AnimateScoreUp());
    }

    /// <summary>
    /// スコアに応じたランクを判定し、色・エフェクトを設定する
    /// </summary>
    private void ApplyRankEffects()
    {
        if (scoreRanks == null || scoreRanks.Count == 0) return;

        // 要求スコアが高い順（降順）にソートして、条件を満たす一番高いランクを探す
        var sortedRanks = scoreRanks.OrderByDescending(r => r.minScore).ToList();

        // デフォルトとして一番低いランクを入れておく
        currentRank = sortedRanks[sortedRanks.Count - 1];

        foreach (var rank in sortedRanks)
        {
            if (currentScore >= rank.minScore)
            {
                currentRank = rank;
                break; // 一番高いランクが見つかったら終了
            }
        }

        if (currentRank == null) return;

        Color safeColor = Palette.ColorA;
        if (currentRank.minScore > 10) safeColor = Palette.ColorB;
        if (currentRank.minScore > 30) safeColor = Palette.ColorC;
        safeColor.a = 1f;

        // 1. 色の変更
        if (resultScoreTextTMP != null) resultScoreTextTMP.color = safeColor;
        if (resultScoreText != null)    resultScoreText.color    = safeColor;

        // 2. エフェクトの生成
        if (currentRank.effectPrefab != null)
        {
            // エフェクトを画面中央（または特定の場所）に生成する
            Instantiate(currentRank.effectPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void Update()
    {
        // --- テキストのアニメーション（揺れ） ---
        if (scoreRectTransform != null && currentRank != null && currentRank.shakeAmount > 0f)
        {
            // 時間とSin波、ランダムを組み合わせて激しく震えさせる
            float t = Time.time * currentRank.shakeSpeed;
            float shakeX = Mathf.Sin(t) * currentRank.shakeAmount + Random.Range(-currentRank.shakeAmount, currentRank.shakeAmount) * 0.2f;
            float shakeY = Mathf.Cos(t * 1.3f) * currentRank.shakeAmount + Random.Range(-currentRank.shakeAmount, currentRank.shakeAmount) * 0.2f;

            scoreRectTransform.anchoredPosition = initialScorePosition + new Vector2(shakeX, shakeY);
        }

        // --- Tap to Title の点滅アニメーション ---
        float alpha = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f; // 0.0 ~ 1.0を波打つ
        alpha = Mathf.Lerp(0.3f, 1.0f, alpha); // 0.3 ~ 1.0の間で点滅させる
        if (tapToTitleTextTMP != null && tapToTitleTextTMP.gameObject.activeSelf) 
        {
            // コルーチンが終わってアルファが設定されてからフェードさせるようにするため
            // 最初はStartで0fにしているが、アニメーション開始後はこのUpdateで上書きされる
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

        // --- シーン遷移処理 ---
        if (!isTransitioning && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    /// <summary>
    /// スコアを0から実際の値までカウントアップするアニメーション
    /// </summary>
    private IEnumerator AnimateScoreUp()
    {
        // Tap To Title を最初は隠しておく（Startで設定済みだが念のため）
        
        float duration = 0.8f; // 0.8秒かけてカウントアップ
        float elapsed = 0f;
        
        // 少し待ってからカウント開始
        yield return new WaitForSeconds(0.2f);
        
        if (AudioManager.Instance != null)
        {
            // ドラムロール的な音があればここで鳴らすのもあり
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // イーズアウト（後半ゆっくりになる）
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            int displayScore = Mathf.RoundToInt(currentScore * easeOut);
            
            if (resultScoreTextTMP != null) resultScoreTextTMP.text = displayScore.ToString();
            if (resultScoreText != null)    resultScoreText.text    = displayScore.ToString();
            
            // スコアがカウントアップしている間、少しスケールを揺らす
            if (scoreRectTransform != null)
            {
                scoreRectTransform.localScale = Vector3.one * (1f + Random.Range(-0.05f, 0.05f));
            }
            
            yield return null;
        }
        
        // 最終スコアを確実にセット
        if (resultScoreTextTMP != null) resultScoreTextTMP.text = currentScore.ToString();
        if (resultScoreText != null)    resultScoreText.text    = currentScore.ToString();
        
        // 最後にスケールをポンッと大きくして戻す演出
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

        // Tap To Title の表示を許可（Updateの点滅に引き継がれる）
        if (tapToTitleTextTMP != null) tapToTitleTextTMP.alpha = 1f;
        if (tapToTitleText != null)
        {
            var c = tapToTitleText.color;
            c.a = 1f;
            tapToTitleText.color = c;
        }
    }

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

