using System.Collections;
using UnityEngine;

/// <summary>
/// 消えた円の位置から次の出現位置へ向かって線を伸ばすエフェクト。
/// コルーチンで「伸びる → 保持 → フェードアウト → 自己破棄」の3フェーズを実行する。
///
/// 【使い方】
/// 1. 空の GameObject にこのスクリプトをアタッチしてプレハブ化する
///    （LineRenderer は [RequireComponent] で自動追加される）
/// 2. GameManager の「Connect Line Prefab」フィールドにアサインする
/// 3. 各パラメータは Inspector またはコードのデフォルト値で調整する
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ConnectLineEffect : MonoBehaviour
{
    // -------------------------------------------------------
    // Inspector 公開パラメータ
    // -------------------------------------------------------

    [Header("アニメーション時間（秒）")]
    // 線が始点から終点へ伸びるのにかかる時間
    [SerializeField] private float growDuration = 0.12f;
    // 完全に伸びてから消え始めるまでの待機時間
    [SerializeField] private float holdDuration = 0.05f;
    // フェードアウトにかかる時間
    [SerializeField] private float fadeDuration = 0.18f;

    [Header("線のスタイル")]
    // 始点側（消えた円）の太さ
    [SerializeField] private float startWidth = 0.06f;
    // 終点側（次の出現位置）の太さ（細くして矢印のような印象に）
    [SerializeField] private float endWidth   = 0.02f;
    // 始点の色（alpha が初期透明度になる）
    [SerializeField] private Color startColor = new Color(1.0f, 0.85f, 0.2f, 0.9f);
    // 終点の色（薄めにして方向感を出す）
    [SerializeField] private Color endColor   = new Color(1.0f, 0.85f, 0.2f, 0.3f);

    // -------------------------------------------------------
    // 内部変数
    // -------------------------------------------------------
    private LineRenderer lineRenderer;

    // -------------------------------------------------------
    // Unity ライフサイクル
    // -------------------------------------------------------
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    // LineRenderer の基本設定をコードで行う（Inspector での手動設定不要）
    private void SetupLineRenderer()
    {
        // "Sprites/Default" シェーダーを使えば alpha が反映される
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Paletteを使用して色を統一
        startColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.9f);
        endColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.3f);

        // --- LineRenderer の初期設定 ---
        lineRenderer.positionCount    = 2;
        lineRenderer.startWidth       = startWidth;
        lineRenderer.endWidth         = endWidth;
        lineRenderer.useWorldSpace    = true;   // ワールド座標で始点・終点を指定する
        lineRenderer.startColor       = startColor;
        lineRenderer.endColor         = endColor;
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder     = 10;     // Circle より前面に描画する
    }

    // -------------------------------------------------------
    // 公開メソッド
    // -------------------------------------------------------

    /// <summary>
    /// 線のアニメーションを開始する。
    /// GameManager.SpawnNextCircle() 内部から呼ばれる。
    /// </summary>
    /// <param name="from">始点（タップされた円の座標）</param>
    /// <param name="to">終点（次の円の出現座標）</param>
    public void Play(Vector3 from, Vector3 to)
    {
        StartCoroutine(AnimateLine(from, to));
    }

    // -------------------------------------------------------
    // コルーチン（3フェーズのアニメーション）
    // -------------------------------------------------------
    private IEnumerator AnimateLine(Vector3 from, Vector3 to)
    {
        // ===== フェーズ1: 線を from → to に向かって伸ばす =====
        float elapsed = 0f;
        lineRenderer.SetPosition(0, from); // 始点は固定
        lineRenderer.SetPosition(1, from); // 最初は長さ0（始点と同じ位置）

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            // EaseOut: 最初は速く、終点に近づくにつれてゆっくりになる
            float t = Mathf.Clamp01(elapsed / growDuration);
            t = 1f - (1f - t) * (1f - t);
            lineRenderer.SetPosition(1, Vector3.Lerp(from, to, t));
            yield return null;
        }
        lineRenderer.SetPosition(1, to); // 終点をぴったり合わせる

        // ===== フェーズ2: 完全に伸びた状態で少し待機 =====
        yield return new WaitForSeconds(holdDuration);

        // ===== フェーズ3: alpha を 1→0 にフェードアウト =====
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            // 始点・終点それぞれの alpha を元の値に掛け合わせてフェード
            lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b,
                                                startColor.a * alpha);
            lineRenderer.endColor   = new Color(endColor.r,   endColor.g,   endColor.b,
                                                endColor.a   * alpha);
            yield return null;
        }

        // アニメーション完了後に自己破棄
        Destroy(gameObject);
    }
}
