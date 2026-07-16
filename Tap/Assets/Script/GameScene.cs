using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム全体の進行管理（開始・終了・シーン遷移）を担当する司令塔クラス。
/// スコア処理は ScoreManager へ、タイマー処理は TimerManager へ委譲する。
///
/// 【Inspectorでの設定方法】
/// (1) ScoreManager コンポーネントをアタッチした GameObject を scoreManager フィールドにアサインする。
/// (2) TimerManager コンポーネントをアタッチした GameObject を timerManager フィールドにアサインする。
///     ※ 同一 GameObject にまとめてアタッチしても、別々の GameObject に分けてもどちらでも可。
/// (3) circlePrefab に生成する円のプレハブをアサインする。
/// (4) gameOverUI にゲームオーバー時に表示するパネルをアサインする。
/// (5) fadeImage・fadeDuration・nextSceneName でシーン遷移のフェード設定を行う。
/// </summary>
public class GameScene : MonoBehaviour
{
    // --- 円の生成設定 ---
    [Header("円の生成設定")]
    [SerializeField] private GameObject circlePrefab;

    // Circleプレハブの半径（画面内に収まるようこの分だけ内側に制限する）
    // Circleプレハブの実際の大きさに合わせてInspectorで調整すること
    [SerializeField] private float circleRadius = 0.5f;

    [Header("画面端からの余白（UIと被らないように調整）")]
    [SerializeField] private float paddingBottom = 2.0f; // 下部のUIなどを避けるための余白
    [SerializeField] private float paddingTop    = 0.5f;
    [SerializeField] private float paddingLeft   = 0.5f;
    [SerializeField] private float paddingRight  = 0.5f;

    // スポーン可能な画面内の範囲（Start()でCameraから自動計算される）
    private float spawnMinX, spawnMaxX;
    private float spawnMinY, spawnMaxY;

    // 次のCircle出現位置に表示する薄い輪エフェクトのプレハブ
    // SpawnHintEffect スクリプトをアタッチした空のGameObjectをプレハブ化してアサインする
    [SerializeField] private GameObject spawnHintPrefab;

    // 消えた円 → 次の出現位置へ向かう線エフェクトのプレハブ
    // ConnectLineEffect スクリプトをアタッチした空のGameObjectをプレハブ化してアサインする
    [SerializeField] private GameObject spawnConnectLinePrefab;

    [Header("エフェクト設定")]
    [Tooltip("スコアが上がった時に画面両端から出る「びりびり」エフェクト")]
    [SerializeField] private GameObject sideZapEffectPrefab;

    // --- ゲームオーバーUI ---
    // Inspectorでゲームオーバー時に表示するパネルをアサインする
    [Header("ゲームオーバーUI")]
    [SerializeField] private GameObject gameOverUI;

    // --- フェード設定 ---
    [Header("フェード設定")]
    [SerializeField] private Image  fadeImage;
  //  [SerializeField] private float  fadeDuration  = 1f;
    [SerializeField] private string nextSceneName = "Result";

    // --- 各 Manager への参照 ---
    // InspectorでそれぞれのManagerコンポーネントをアサインする
    [Header("各 Manager（Inspectorでアサイン）")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimerManager timerManager;

    // --- カウントダウンUI ---
    [Header("カウントダウンUI")]
    [Tooltip("ゲーム開始前のカウントダウンを表示する TextMeshPro")]
    [SerializeField] private TMPro.TMP_Text countdownTextTMP;
    [Tooltip("ゲーム開始前のカウントダウンを表示する 従来の Text")]
    [SerializeField] private Text countdownText;

    // --- 内部状態（ステートマシン） ---
    public enum GameState
    {
        Countdown,
        Playing,
        GameOver,
        Transitioning
    }

    /// <summary>現在のゲームの状態</summary>
    public GameState CurrentState { get; private set; }

    /// <summary>外部クラス（CircleController等）からの後方互換用プロパティ</summary>
    public bool isGameActive => CurrentState == GameState.Playing;

    /// <summary>現在のスコアを取得する</summary>
    public int CurrentScore => scoreManager != null ? scoreManager.Score : 0;

    [Header("BGM設定")]
    [Tooltip("GameSceneで流すBGM（未設定の場合は自動でGameBGMを読み込みます）")]
    [SerializeField] private AudioClip bgmClip;


    void Start()
    {
        // --- 背景画像の設定 ---
        BackgroundHelper.SetupBackground("bg_game");

        // BGMはカウントダウン完了後（GO!のタイミング）に再生するため、ここでは準備のみ
        if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Audio/GameBGM");

        // 初期状態はカウントダウン
        CurrentState = GameState.Countdown;

        // ゲームオーバーUIを非表示にして初期化
        if (gameOverUI != null) gameOverUI.SetActive(false);

        // フェードイン準備（画面を透明な黒から始める）
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0, 0, 0, 0);
        }

        // TimerManager のコールバックだけ先に登録しておく（計測開始は後）
        if (timerManager != null)
        {
            timerManager.SetOnTimeUp(GameOver);
        }

        // 画面のワールド座標範囲を計算する（円が画面内に収まるようにする）
        CalculateSpawnBounds();

        // カウントダウン処理をスタート
        StartCoroutine(CountdownRoutine());
    }

    /// <summary>
    /// 3, 2, 1, START! とカウントダウンしてからゲームを本格的に開始するコルーチン
    /// </summary>
    private IEnumerator CountdownRoutine()
    {
        string[] counts = {  "Ready" }; // カウントダウンを少しリッチに
        
        foreach (string c in counts)
        {
            if (countdownTextTMP != null) countdownTextTMP.text = c;
            if (countdownText != null)    countdownText.text = c;
            // カウントダウン音「ピッ」
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownTick();

            // アニメーション: 大きくしてから元のサイズへ
            yield return StartCoroutine(AnimateText(1f));
        }

        // スタート表示
        if (countdownTextTMP != null) countdownTextTMP.text = " GO!";

        // ゲーム開始音「ポーン！」
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownGo();

        // --- ここでBGMを再生開始 ---
        if (AudioManager.Instance != null && bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(bgmClip, 0.4f);
        }

        yield return StartCoroutine(AnimateText(0.5f, 1.5f));

        // カウントダウンテキストを非表示にする
        if (countdownTextTMP != null) countdownTextTMP.gameObject.SetActive(false);


        // --- ここから本格的にゲーム開始 ---
        
        // スコアとタイマーの初期化＆計測開始
        if (scoreManager != null) scoreManager.Initialize();
        if (timerManager != null) timerManager.Initialize();



        // プレイヤーの操作を受け付ける状態（Playing）に変更
        CurrentState = GameState.Playing;

        // 最初の円を生成する
        SpawnNextCircle();
    }

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
            // 0 -> 1 の間でサイン波を使ってスケールを大きくして戻す
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * (maxScale - 1f);
            targetTransform.localScale = Vector3.one * scale;
            yield return null;
        }
        targetTransform.localScale = Vector3.one;
    }


    /// <summary>
    /// Camera のワールド座標から、円が画面内に完全に収まるスポーン範囲を計算してキャッシュする。
    /// 解像度・アスペクト比が異なるデバイスでも自動的に正しい範囲が設定される。
    /// Start() から呼ぶこと。
    /// </summary>
    private void CalculateSpawnBounds()
    {
        Camera cam = Camera.main;

        // カメラから円が存在するワールドZ=0 平面までの距離を求める
        // 2Dゲームでは Camera.main は通常 Z=-10 に配置されているため距離は 10
        float dist = Mathf.Abs(cam.transform.position.z);
        if (dist < 0.01f) dist = 10f; // カメラがZ=0にある場合のフォールバック

        // ビューポート座標：(0,0) = 画面左下、(1,1) = 画面右上 をワールド座標に変換
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0f, dist));
        Vector3 topRight   = cam.ViewportToWorldPoint(new Vector3(1f, 1f, dist));

        // Circleプレハブの実際の半径をコンポーネントから自動取得
        float halfSize = GetCircleHalfSize();

        // 半径 + Inspectorで設定した各方向のパディング分だけ内側に制限する
        spawnMinX = bottomLeft.x + halfSize + paddingLeft;
        spawnMaxX = topRight.x   - halfSize - paddingRight;
        spawnMinY = bottomLeft.y + halfSize + paddingBottom;
        spawnMaxY = topRight.y   - halfSize - paddingTop;

      //  Debug.Log($"[GameManager] Circle半径={halfSize:F2} | SpawnBounds: X({spawnMinX:F2}〜{spawnMaxX:F2})  Y({spawnMinY:F2}〜{spawnMaxY:F2})");
    }

    /// <summary>
    /// circlePrefab のコンポーネントからスケール込みの半径を自動取得する。
    /// 取得優先順位: CircleCollider2D → SpriteRenderer → Inspector手動値(circleRadius)
    /// </summary>
    private float GetCircleHalfSize()
    {
        if (circlePrefab == null) return circleRadius;

        // ① CircleCollider2D がある場合（最も正確）
        // radius × scale.x でワールドスケールの半径が得られる
        CircleCollider2D col2D = circlePrefab.GetComponent<CircleCollider2D>();
        if (col2D != null)
            return col2D.radius * circlePrefab.transform.localScale.x;

        // ② SpriteRenderer がある場合
        // sprite の bounds から半径を推定する
        SpriteRenderer sr = circlePrefab.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            return sr.sprite.bounds.extents.x * circlePrefab.transform.localScale.x;

        // ③ 上記どちらもなければ Inspector の circleRadius をフォールバックとして使用
       // Debug.LogWarning("[GameManager] Circle の半径を自動取得できませんでした。circleRadius の値を使用します。");
        return circleRadius;
    }

    void Update()
    {
        // 自動でシーン遷移するようにしたため、Updateでのタップ待ちは削除
    }

    /// <summary>
    /// 次の円をランダムな位置に生成する。
    /// 引数なし: Start()から呼ばれる（最初の円なので線は不要）。
    /// fromPosition 持ち: CircleControllerからタップ時に呼ばれる（線エフェクトあり）。
    /// </summary>
    /// <param name="fromPosition">前の円の座標（なければ線エフェクトは生成しない）</param>
    public Vector3 SpawnNextCircle(Vector3? fromPosition = null)
    {
        if (!isGameActive) return Vector3.zero;

        // 画面内の範囲内でランダムな座標を决定する
        // spawnMin/Max は CalculateSpawnBounds() でカメラのワールド座標から計算済み
        float randomX = Random.Range(spawnMinX, spawnMaxX);
        float randomY = Random.Range(spawnMinY, spawnMaxY);
        Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

        // --- 線エフェクト ---
        // fromPosition が渡された時のみ実行（最初の円生成時はスキップ）
        if (fromPosition.HasValue && spawnConnectLinePrefab != null)
        {
            // 線エフェクトのプレハブを生成し、Play()でアニメーション開始
            // オブジェクトは ConnectLineEffect 内部で自己破棄されるので外から管理不要
            GameObject lineObj = Instantiate(spawnConnectLinePrefab, Vector3.zero, Quaternion.identity);
            ConnectLineEffect lineEffect = lineObj.GetComponent<ConnectLineEffect>();
            if (lineEffect != null)
                lineEffect.Play(fromPosition.Value, spawnPos);
        }

        // --- 輪エフェクト（出現位置の目印） ---
        // spawnHintPrefab は Inspector でアサインしておく（未設定なら何もしない）
        if (spawnHintPrefab != null)
            Instantiate(spawnHintPrefab, spawnPos, Quaternion.identity);

        // --- Circle 本体の生成 ---
        GameObject circleObj = Instantiate(circlePrefab, spawnPos, Quaternion.identity);
        
        // 1. 円の見た目を1.3倍に大きくする
        circleObj.transform.localScale *= 1.3f;
        
        // 2. タップ判定（当たり判定）を見かけよりもさらに少し大きくして押しやすくする（+20%）
        CircleCollider2D col = circleObj.GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.radius *= 1.2f;
        }

        return spawnPos;
    }

    /// <summary>
    /// スコアを加算する。
    /// CircleController からタップ時に呼ばれ、内部で ScoreManager に委譲する。
    /// </summary>
    /// <param name="points">加算するポイント数</param>
    public int AddScore(int points)
    {
        if (!isGameActive) return 0;

        int added = 0;
        // ScoreManager に実際の加算処理を任せる
        if (scoreManager != null) added = scoreManager.AddScore(points);

        return added;
    }


    /// <summary>
    /// ゲームオーバー処理。TimerManager のタイムアップコールバックから呼ばれる。
    /// </summary>
    private void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.GameOver;

        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlayMissSound();
            // 音が急に止まると不自然なので、0.8秒かけて滑らかにフェードアウトする
            AudioManager.Instance.FadeOutBGM(0.5f);
        }

        // タイマーを停止する（念のため二重停止を防ぐ）
        if (timerManager != null) timerManager.Stop();

        // 画面上にあるCircleを直ちに非表示（削除）する
        CircleController[] circles = Object.FindObjectsOfType<CircleController>();
        foreach(var c in circles)
        {
            if (c != null && c.gameObject != null)
            {
                c.gameObject.SetActive(false);
            }
        }

        // スコアを PlayerPrefs に保存し、最終スコアUIを表示する
        if (scoreManager != null) scoreManager.SaveAndShowFinalScore();

        // 時間の流れを元に戻す（スローモーションなどが入っていた場合への対策）
        Time.timeScale = 1f;

        // すぐにResultSceneへ移行する（フリーズ感を完全になくす）
        StartCoroutine(FastTransitionToResult());
    }

    private IEnumerator FastTransitionToResult()
    {
        // TimeUp のUIを一瞬だけ出す（不要な場合は削除可能ですが、一応表示だけします）
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            gameOverUI.transform.localScale = Vector3.one;
            CanvasGroup cg = gameOverUI.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }

        CurrentState = GameState.Transitioning;

        // 0.5秒のフェードアウトでシーンを切り替える
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