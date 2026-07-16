using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトル画面（TitleScene）の制御を行うスクリプト。
/// ロゴをふわふわアニメーションさせたり、画面タップでゲーム画面へ遷移させたりします。
/// </summary>
public class TitleScene : MonoBehaviour
{
    // --- 定数 ---
    // 遷移先のシーン名を定数で定義しておくことで、打ち間違いを防ぎます
    private const string NEXT_SCENE = "Game";

    // --- ロゴアニメーション設定 ---
    [Header("タイトルロゴのアニメーション設定")]
    [Tooltip("動かしたいロゴの Transform (座標などの情報)")]
    [SerializeField] private Transform titleLogo;
    [Tooltip("ロゴが上下に動く速さ")]
    [SerializeField] private float moveSpeed  = 2f;
    [Tooltip("ロゴが上下に動く幅（大きさ）")]
    [SerializeField] private float moveAmount = 20f;

    // --- フェード設定 ---
    [Header("フェード設定")]
    [Tooltip("画面を暗くするための黒い画像")]
    [SerializeField] private Image  fadeImage;
    [Tooltip("フェードアウトにかかる時間（秒）")]
    [SerializeField] private float  fadeDuration  = 1f;
    [Tooltip("次に読み込むシーンの名前")]
    [SerializeField] private string nextSceneName = NEXT_SCENE;

    // --- BGM設定 ---
    [Header("BGM設定")]
    [Tooltip("タイトルで流すBGM（未設定の場合は自動でTItleBGMを読み込みます）")]
    [SerializeField] private AudioClip bgmClip;

    // --- 内部変数 ---
    // ロゴの最初の位置を覚えておくための変数
    private Vector3 initialLogoPosition;
    // シーン遷移中かどうかを判定するフラグ（連続タップ防止）
    private bool isTransitioning;

    private void Start()
    {
        // --- 背景画像の設定 ---
        BackgroundHelper.SetupBackground("bg_title");

        // --- BGMの再生 ---
        if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Audio/TItleBGM");
        if (AudioManager.Instance != null && bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(bgmClip, 0.4f); // 音量は0.4f（少し控えめ）
        }

        // ロゴが設定されていれば、最初の位置を記録する
        if (titleLogo != null)
            initialLogoPosition = titleLogo.localPosition;

        // フェード用の黒い画像を最初に透明にしておく
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0f, 0f, 0f, 0f); // 最後の 0f が透明度 (Alpha)
        }
    }

    private void Update()
    {
        // --- ロゴのふわふわアニメーション ---
        // まだシーン遷移中でなければ、ロゴを上下に動かす
        if (titleLogo != null && !isTransitioning)
        {
            // 時間 (Time.time) と Sin (サイン波) を使って、滑らかな上下運動の Y 座標を計算する
            float newY = initialLogoPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
            titleLogo.localPosition = new Vector3(initialLogoPosition.x, newY, initialLogoPosition.z);
        }

        // --- シーン遷移処理 ---
        // マウスの左クリック（スマホのタップ）がされたら、ゲーム画面へ遷移する
        if (Input.GetMouseButtonDown(0) && !isTransitioning)
        {
            // コルーチンを使ってフェード処理を開始
            StartCoroutine(FadeAndLoadScene());
        }
    }

    /// <summary>
    /// 画面を徐々に暗くして、次のシーンを読み込むコルーチン
    /// </summary>
    private IEnumerator FadeAndLoadScene()
    {
        // 遷移中フラグを立てて、Updateでの入力を受け付けなくする
        isTransitioning = true;

        if (fadeImage != null)
        {
            float elapsed = 0f;
            // 経過時間が fadeDuration（フェード時間）に達するまでループする
            while (elapsed < fadeDuration)
            {
                // 1フレームの経過時間を足す
                elapsed += Time.deltaTime;
                
                // 経過時間の割合 (0.0 〜 1.0) を計算して、画像の透明度を徐々に上げる (暗くする)
                fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(elapsed / fadeDuration));
                
                // 次のフレームまで待機する
                yield return null;
            }
        }

        // フェードが終わったらシーンを読み込む
        // 自作の SceneManager があればそれを使用し、なければ Unity 標準のものを使う
        if (SceneManager.Instance != null)
            SceneManager.Instance.LoadScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}

