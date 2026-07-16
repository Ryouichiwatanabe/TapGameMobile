using UnityEngine;
using System.Collections;

/// <summary>
/// 画面上に表示される円（ターゲット）の挙動を制御するスクリプト。
/// クリック（タップ）された時のスコア加算、エフェクト生成、次の円の生成などを担当します。
/// </summary>
public class CircleController : MonoBehaviour
{
    [Header("エフェクト設定")]
    [Tooltip("円をタップした時に再生するパーティクルやエフェクトのプレハブ")]
    [SerializeField] private GameObject clickEffectPrefab;

    [Header("カラーアニメーション設定")]
    [Tooltip("円の色の変化を設定するグラデーション")]
    [SerializeField] private Gradient colorGradient;
    [Tooltip("色が変化するスピード")]
    [SerializeField] private float colorChangeSpeed = 1f;

    // --- 内部変数 ---
    // ゲーム全体の進行を管理する GameScene への参照
    private GameScene gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isDying = false;

    private void Start()
    {
        // シーン内にある GameScene コンポーネントを自動的に探し出して取得する
        // ※ Object.FindAnyObjectByType は少し重い処理ですが、Start時の1回だけなら問題ありません
        gameManager = Object.FindAnyObjectByType<GameScene>();

        // 自分の色を変えるために SpriteRenderer を取得
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Paletteを使って色を統一
        colorGradient = Palette.GetMainGradient();

        // 出現アニメーション（Pop-in）
        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        Vector3 targetScale = transform.localScale;
        transform.localScale = Vector3.zero;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // イーズアウトバックのような弾む動き
            float t = elapsed / duration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) + Mathf.Sin(t * Mathf.PI) * 0.2f;
            transform.localScale = targetScale * scale;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private void Update()
    {
        if (isDying) return;

        // 毎フレーム色をグラデーションに従って変化させる
        if (spriteRenderer != null && colorGradient != null)
        {
            // 時間経過で 0.0 〜 1.0 の間を行ったり来たり（PingPong）させる
            float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
            spriteRenderer.color = colorGradient.Evaluate(t);
        }
    }

    /// <summary>
    /// マウスでクリック（またはスマホでタップ）された瞬間に呼ばれる Unity 組み込みのメソッド。
    /// オブジェクトに Collider (当たり判定) が付いている必要があります。
    /// </summary>
    private void OnMouseDown()
    {
        // GameManager が見つからない、ゲームがすでに終了している、または消滅アニメーション中なら何もしない
        if (gameManager == null || !gameManager.isGameActive || isDying) return;
        isDying = true;

        // --- タップ成功時の処理 ---

        // 1. スコアを1点加算する
        int addedScore = gameManager.AddScore(1);

        // --- タップ音（コンボでピッチが上がる）を鳴らす ---
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTapSound(gameManager.CurrentScore);
        }

        // --- 画面を揺らして光らせる（カッコいい演出） ---
        if (GameJuiceManager.Instance != null)
        {
            GameJuiceManager.Instance.PlayHitEffect(gameManager.CurrentScore);
        }

        // 2. 次の円を生成し、その位置を取得する
        Vector3 nextCirclePos = gameManager.SpawnNextCircle(transform.position);

        // コライダーを無効化してこれ以上押せないようにする
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 4. 爽快感アップのアニメーションを開始して消える
        StartCoroutine(DeathAnimation(nextCirclePos));
    }

    private IEnumerator DeathAnimation(Vector3 nextCirclePos)
    {
        // 爽快感アップのエフェクトを自動生成（Perfectの文字だけ残す）
        CreatePerfectText();

        // 自身が少し膨らんでから消えるアニメーション
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.5f; // 1.5倍に膨らむ

        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            }
            yield return null;
        }

        // 自身の表示を消す
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 円が消えた直後にエフェクトを生成
        if (clickEffectPrefab != null)
        {
            // 次の円に向かってエフェクトを向けるための回転を計算
            Vector3 direction = nextCirclePos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // 2D（Z軸回転）で方向を合わせる。必要に応じて角度のオフセット（-90など）を調整する
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject effect = Instantiate(clickEffectPrefab, transform.position, rotation);
            
            // 円のサイズに合わせてエフェクトの大きさを調整（少し小さめにするなら0.8fなど）
            effect.transform.localScale = transform.localScale * 0.8f;
            
            ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                // 再生速度を上げて、表示時間を短く（スピーディに）する
                var main = ps.main;
                main.simulationSpeed = 2.0f; // 2倍速
                
                var r = ps.GetComponent<ParticleSystemRenderer>();
                if (r != null)
                {
                    r.sortingLayerName = "Default";
                    r.sortingOrder = 100;
                }
            }
            
            Destroy(effect, 1.5f);
        }

        // エフェクトのコルーチン（最大0.5秒）が最後まで再生されるように待機する
        yield return new WaitForSeconds(0.6f);

        Destroy(gameObject);
    }

    /// <summary>
    /// 広がって消えるリング（衝撃波）エフェクトをプログラムで生成する
    /// </summary>
    private void CreateRippleEffect()
    {
        GameObject rippleObj = new GameObject("Ripple");
        rippleObj.transform.position = transform.position;
        LineRenderer lr = rippleObj.AddComponent<LineRenderer>();
        
        int segments = 36;
        lr.positionCount = segments + 1;
        lr.useWorldSpace = false;
        lr.startWidth = 0.3f; // 少し太め
        lr.endWidth = 0f;
        
        // 2D用の標準マテリアルを使用
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        lr.endColor = new Color(lr.startColor.r, lr.startColor.g, lr.startColor.b, 0);

        float radius = 1f;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * 360f / segments);
            lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
        }

        StartCoroutine(AnimateRipple(rippleObj, lr));
    }

    private IEnumerator AnimateRipple(GameObject obj, LineRenderer lr)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 2.5f; // 大きく広がる

        while (elapsed < duration)
        {
            if (obj == null) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // イーズアウト（徐々に遅くなる）で広がる
            float easeOut = 1f - (1f - t) * (1f - t);
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, easeOut);
            
            // フェードアウト
            Color color = lr.startColor;
            color.a = Mathf.Lerp(1f, 0f, t);
            lr.startColor = color;

            yield return null;
        }
        Destroy(obj);
    }

    /// <summary>
    /// 飛び散る小さな火花パーティクルをプログラムで生成する
    /// </summary>
    private void CreateSparks()
    {
        GameObject sparksObj = new GameObject("Sparks");
        // 生成時に自動再生されてduration変更時にエラーが出るのを防ぐため、一時的に非アクティブにする
        sparksObj.SetActive(false);
        sparksObj.transform.position = transform.position;
        ParticleSystem ps = sparksObj.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.duration = 0.5f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);
        main.startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        main.loop = false;
        main.playOnAwake = true;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        // 一瞬で多めのパーティクルを放出
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)20) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // 竜巻や渦のように回転（スピン）しながら広がる
        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        // Particle Orbital Velocity curves must all be in the same mode のエラー回避のため、
        // XとYにも同じモード（TwoConstants）のダミー値を設定する
        vel.orbitalX = new ParticleSystem.MinMaxCurve(0f, 0f);
        vel.orbitalY = new ParticleSystem.MinMaxCurve(0f, 0f);
        vel.orbitalZ = new ParticleSystem.MinMaxCurve(5f, 15f); // ぐるぐる回る
        vel.radial = new ParticleSystem.MinMaxCurve(2f, 5f);    // 外側に広がる

        // 光の軌跡（尾）を引かせる
        var trails = ps.trails;
        trails.enabled = true;
        trails.ratio = 1f; // 全てに軌跡をつける
        trails.dieWithParticles = true;
        trails.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, 0f);
        trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, 0.5f), new Color(1f, 1f, 1f, 0f));

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        
        // プログラムで「星（キラキラ）」のテクスチャを生成して割り当てる
        Texture2D starTex = CreateStarTexture();
        Material starMat = new Material(Shader.Find("Sprites/Default"));
        starMat.mainTexture = starTex;
        
        renderer.material = starMat;
        renderer.trailMaterial = new Material(Shader.Find("Sprites/Default")); // 軌跡は普通の線

        // 設定が終わったらアクティブにして再生開始
        sparksObj.SetActive(true);

        Destroy(sparksObj, 1f);
    }

    /// <summary>
    /// キラキラ（星型）のテクスチャをプログラムで生成する
    /// </summary>
    private Texture2D CreateStarTexture()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float u = (x / (float)size) * 2f - 1f;
                float v = (y / (float)size) * 2f - 1f;
                
                // 十字の光の計算
                float alpha = 1f - Mathf.Clamp01(Mathf.Abs(u * v) * 15f); 
                // 円形にフェードアウトさせて自然な形に
                float dist = Mathf.Sqrt(u * u + v * v);
                alpha *= Mathf.Clamp01(1f - dist);
                
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Max(0f, alpha)));
            }
        }
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 消える時に「Perfect!」の文字が飛び出す
    /// </summary>
    private void CreatePerfectText()
    {
        GameObject textObj = new GameObject("PerfectText");
        textObj.transform.position = transform.position + Vector3.up * 0.3f;
        
        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = "";
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.characterSize = 0.1f;
        tm.fontSize = 80;
        tm.color = spriteRenderer != null ? spriteRenderer.color : Color.white;

        // UIより前に表示するためのレイヤー設定
        MeshRenderer mr = textObj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "Default";
            mr.sortingOrder = 50;
        }

        StartCoroutine(AnimatePerfectText(textObj, tm));
    }

    private IEnumerator AnimatePerfectText(GameObject obj, TextMesh tm)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 1.0f; // 少し上にフワッと浮く
        
        Vector3 startScale = Vector3.zero;
        Vector3 peakScale = Vector3.one * 1.2f;

        while (elapsed < duration)
        {
            if (obj == null) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // イーズアウトで上に移動
            float easeOut = 1f - (1f - t) * (1f - t);
            obj.transform.position = Vector3.Lerp(startPos, endPos, easeOut);

            // ポップアップして消えるスケール変化と透明度
            if (t < 0.3f)
            {
                // ポップイン（拡大）
                obj.transform.localScale = Vector3.Lerp(startScale, peakScale, t / 0.3f);
            }
            else
            {
                // ゆっくり元のサイズに戻りながらフェードアウト
                float fadeT = (t - 0.3f) / 0.7f;
                obj.transform.localScale = Vector3.Lerp(peakScale, Vector3.one, fadeT);
                
                Color c = tm.color;
                c.a = Mathf.Lerp(1f, 0f, fadeT);
                tm.color = c;
            }

            yield return null;
        }
        Destroy(obj);
    }
}