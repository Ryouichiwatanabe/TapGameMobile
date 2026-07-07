using UnityEngine;
using System.Collections;

/// <summary>
/// ゲームを「カッコよく」「気持ちよく」するための演出（ジュース）を全自動で追加するクラス。
/// カメラにアタッチするだけで、以下の機能が自動で作動します。
/// 1. 背景のサイバーグリッド（動く網目模様）の自動生成
/// 2. 画面のシェイク（サークルを消した時）
/// 3. 背景のフラッシュ（サークルを消した時）
/// </summary>
public class GameJuiceManager : MonoBehaviour
{
    public static GameJuiceManager Instance;

    [Header("グリッド設定（背景のサイバー空間）")]
    [Tooltip("背景を流れる線の色")]
    public Color gridColor = new Color(0f, 0.8f, 1f, 0.15f); // うっすら光る水色
    [Tooltip("背景が流れるスピード")]
    public float scrollSpeed = 2f;

    [Header("シェイク設定（サークルを消した時の揺れ）")]
    [Tooltip("揺れる時間（秒）")]
    public float shakeDuration = 0.005f;
    [Tooltip("揺れの激しさ")]
    public float shakeMagnitude = 0.1f;

    [Header("フラッシュ設定（サークルを消した時の光）")]
    [Tooltip("フラッシュする際の一瞬の背景色")]
    public Color flashColor = new Color(0.2f, 0.2f, 0.4f); // 少し明るい青

    private Camera cam;
    private Vector3 originalCameraPos;
    private Color originalBackgroundColor;
    private GameObject gridContainer;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            originalCameraPos = cam.transform.localPosition;
            originalBackgroundColor = cam.backgroundColor;
        }

        // Paletteを使って色を統一
        gridColor = new Color(Palette.ColorA.r, Palette.ColorA.g, Palette.ColorA.b, 0.15f);
        flashColor = new Color(Palette.ColorA.r * 0.5f, Palette.ColorA.g * 0.5f, Palette.ColorA.b * 0.5f); // 暗めの同系色

        // --- サイバーな背景グリッドを自動生成 ---
        // 背景画像を追加したため、線（グリッド）は描画しないように無効化
        // GenerateCyberGrid();
    }

    /// <summary>
    /// プログラムでLineRendererを使って網目模様を描画します
    /// </summary>
    private void GenerateCyberGrid()
    {
        gridContainer = new GameObject("CyberGrid");
        gridContainer.transform.SetParent(transform); // カメラの子オブジェクトにする
        gridContainer.transform.localPosition = new Vector3(0, 0, 15f); // カメラの奥に配置

        // 2D標準のシンプルなマテリアルを使用
        Material lineMat = new Material(Shader.Find("Sprites/Default"));
        int gridSize = 15;
        float spacing = 2.0f;

        // 縦線と横線を生成
        for (int i = -gridSize; i <= gridSize; i++)
        {
            // 縦
            CreateLine(new Vector3(i * spacing, -gridSize * spacing, 0), new Vector3(i * spacing, gridSize * spacing, 0), lineMat);
            // 横
            CreateLine(new Vector3(-gridSize * spacing, i * spacing, 0), new Vector3(gridSize * spacing, i * spacing, 0), lineMat);
        }
    }

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
        lr.sortingOrder = -100; // 背景に描画
    }

    private void Update()
    {
        // グリッドをスクロールさせてスピード感を出す
        if (gridContainer != null)
        {
            gridContainer.transform.localPosition += Vector3.down * scrollSpeed * Time.deltaTime;
            
            // 一定距離進んだら元に戻す（無限スクロール）
            if (gridContainer.transform.localPosition.y <= -2.0f)
            {
                gridContainer.transform.localPosition += Vector3.up * 2.0f;
            }
        }
    }

    /// <summary>
    /// サークルを消した時に呼ぶと、画面が揺れて光る！コンボに応じて激しくなる。
    /// </summary>
    public void PlayHitEffect(int combo = 0)
    {
        StopAllCoroutines();
        if (cam != null)
        {
            StartCoroutine(ShakeRoutine(combo));
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator ShakeRoutine(int combo)
    {
        // コンボ数に応じて最大2倍まで揺れを大きくする
        float comboMultiplier = 1f + Mathf.Min(combo, 20) * 0.005f;
        float currentShakeDuration = shakeDuration * comboMultiplier;
        float currentShakeMagnitude = shakeMagnitude * comboMultiplier;

        float elapsed = 0f;
        
        // ランダムな開始オフセットを設けることで、毎回違う揺れ方にする
        float randomOffsetX = Random.Range(0f, 100f);
        float randomOffsetY = Random.Range(0f, 100f);

        while (elapsed < currentShakeDuration)
        {
            elapsed += Time.deltaTime;
            
            // 後半になるにつれて揺れを滑らかに減衰（フェードアウト）させる
            float damping = 1f - (elapsed / currentShakeDuration);
            float currentMag = currentShakeMagnitude * damping;
            
            // Random.Rangeの代わりにPerlinNoiseを使うことで、ガタガタせず滑らかに揺らす
            float noiseX = Mathf.PerlinNoise(randomOffsetX + Time.time * 25f, 0f) * 2f - 1f;
            float noiseY = Mathf.PerlinNoise(0f, randomOffsetY + Time.time * 25f) * 2f - 1f;
            
            float x = noiseX * currentMag;
            float y = noiseY * currentMag;
            
            cam.transform.localPosition = new Vector3(originalCameraPos.x + x, originalCameraPos.y + y, originalCameraPos.z);
            yield return null;
        }
        cam.transform.localPosition = originalCameraPos;
    }

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
    /// 指定した座標に文字を浮かび上がらせる（フローティングテキスト）
    /// </summary>
    public void SpawnFloatingText(Vector3 position, string text, Color color, float scale = 1f)
    {
        // 新しいGameObjectを作成
        GameObject floatObj = new GameObject("FloatingText");
        floatObj.transform.position = position;
        floatObj.transform.localScale = Vector3.one * scale;
        // TextMeshProの追加と設定
        TMPro.TextMeshPro tmp = floatObj.AddComponent<TMPro.TextMeshPro>();
        tmp.text = text;
        tmp.color = color;
        tmp.fontSize = 8f;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.fontStyle = TMPro.FontStyles.Bold;
        // 描画順を最前面にする
        tmp.sortingOrder = 100;

        // アニメーションのコルーチンを開始
        StartCoroutine(FloatingTextRoutine(floatObj, tmp));
    }

    private IEnumerator FloatingTextRoutine(GameObject obj, TMPro.TextMeshPro tmp)
    {
        float duration = 0.8f;
        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f; // 上に移動
        Color startColor = tmp.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // 透明に

        while (elapsed < duration)
        {
            if (obj == null) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // イーズアウト（徐々に遅くなる）
            float easeOut = 1f - (1f - t) * (1f - t);
            
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
