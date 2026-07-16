using UnityEngine;

/// <summary>
/// 次のCircle出現位置に一瞬表示される薄い輪のエフェクト。
/// LineRenderer で円形のアウトラインを描き、すぐにフェードアウトして自己破棄する。
///
/// 【使い方】
/// このスクリプトをアタッチした空のGameObjectをプレハブ化し、
/// GameManager の「Spawn Hint Prefab」フィールドにアサインするだけ。
/// マテリアルなどの設定はコード内で自動的に行う。
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class SpawnHintEffect : MonoBehaviour
{
    // --- 輪のサイズ設定 ---
    // CirclePrefab の見た目の半径に合わせて調整する
    [SerializeField] private float radius    = 0.55f;
    [SerializeField] private int   segments  = 40;       // 多いほど滑らかな円になる

    // --- 線の太さ ---
    [SerializeField] private float lineWidth = 0.04f;

    // --- 色・透明度 ---
    [SerializeField] private Color hintColor = new Color(1f, 1f, 1f, 0.6f);

    // --- フェード時間（秒） ---
    [SerializeField] private float duration  = 0.4f;

    private LineRenderer lineRenderer;
    private float        elapsed;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
        DrawCircle();
    }

    private void SetupLineRenderer()
    {
        // マテリアルをコードで設定（Inspectorでの手動設定不要）
        lineRenderer.material         = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.loop             = true;              // 始点と終点をつなげて閉じた円にする

        // Paletteを使って統一
        hintColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.6f);

        // -- LineRenderer 基本設定 --
        lineRenderer.positionCount    = segments;
        lineRenderer.startWidth       = lineWidth;
        lineRenderer.endWidth         = lineWidth;
        lineRenderer.useWorldSpace    = false;             // ローカル座標で描画
        lineRenderer.startColor       = hintColor;
        lineRenderer.endColor         = hintColor;
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder     = 5;                 // Circleより手前に表示
    }

    // 円形に頂点を配置する
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

    private void Update()
    {
        elapsed += Time.deltaTime;

        // フェードアウト：alpha を 初期値→0 にリニアに変化させる
        float alpha = Mathf.Lerp(hintColor.a, 0f, elapsed / duration);
        Color c     = new Color(hintColor.r, hintColor.g, hintColor.b, alpha);
        lineRenderer.startColor = c;
        lineRenderer.endColor   = c;

        // 時間が過ぎたら自己破棄
        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
