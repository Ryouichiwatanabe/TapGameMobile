using UnityEngine;
using System.Collections;

/// <summary>
/// 逕ｻ髱｢荳翫↓陦ｨ遉ｺ縺輔ｌ繧句・・医ち繝ｼ繧ｲ繝・ヨ・峨・謖吝虚繧貞宛蠕｡縺吶ｋ繧ｹ繧ｯ繝ｪ繝励ヨ縲・/// 繧ｯ繝ｪ繝・け・医ち繝・・・峨＆繧後◆譎ゅ・繧ｹ繧ｳ繧｢蜉邂励√お繝輔ぉ繧ｯ繝育函謌舌∵ｬ｡縺ｮ蜀・・逕滓・縺ｪ縺ｩ繧呈球蠖薙＠縺ｾ縺吶・/// </summary>
public class CircleController : MonoBehaviour
{
    [Header("繧ｨ繝輔ぉ繧ｯ繝郁ｨｭ螳・)]
    [Tooltip("蜀・ｒ繧ｿ繝・・縺励◆譎ゅ↓蜀咲函縺吶ｋ繝代・繝・ぅ繧ｯ繝ｫ繧・お繝輔ぉ繧ｯ繝医・繝励Ξ繝上ヶ")]
    [SerializeField] private GameObject clickEffectPrefab;

    [Header("繧ｫ繝ｩ繝ｼ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ險ｭ螳・)]
    [Tooltip("蜀・・濶ｲ縺ｮ螟牙喧繧定ｨｭ螳壹☆繧九げ繝ｩ繝・・繧ｷ繝ｧ繝ｳ")]
    [SerializeField] private Gradient colorGradient;
    [Tooltip("濶ｲ縺悟､牙喧縺吶ｋ繧ｹ繝斐・繝・)]
    [SerializeField] private float colorChangeSpeed = 1f;

    // --- 蜀・Κ螟画焚 ---
    // 繧ｲ繝ｼ繝蜈ｨ菴薙・騾ｲ陦後ｒ邂｡逅・☆繧・GameScene 縺ｸ縺ｮ蜿ら・
    private GameScene gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isDying = false;

    /// <summary>
    /// 繧ｪ繝悶ず繧ｧ繧ｯ繝育函謌先凾縺ｮ蛻晄悄蛹門・逅・・ameScene縺ｮ蜿門ｾ励ｄ濶ｲ縺ｮ險ｭ螳壹∝・迴ｾ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ縺ｮ髢句ｧ九ｒ陦後≧縲・    /// </summary>
    private void Start()
    {
        // 繧ｷ繝ｼ繝ｳ蜀・↓縺ゅｋ GameScene 繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ閾ｪ蜍慕噪縺ｫ謗｢縺怜・縺励※蜿門ｾ励☆繧・        // 窶ｻ Object.FindAnyObjectByType 縺ｯ蟆代＠驥阪＞蜃ｦ逅・〒縺吶′縲ヾtart譎ゅ・1蝗槭□縺代↑繧牙撫鬘後≠繧翫∪縺帙ｓ
        gameManager = Object.FindAnyObjectByType<GameScene>();

        // 閾ｪ蛻・・濶ｲ繧貞､峨∴繧九◆繧√↓ SpriteRenderer 繧貞叙蠕・        spriteRenderer = GetComponent<SpriteRenderer>();

        // Palette繧剃ｽｿ縺｣縺ｦ濶ｲ繧堤ｵｱ荳
        colorGradient = Palette.GetMainGradient();

        // 蜃ｺ迴ｾ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・・op-in・・        StartCoroutine(SpawnAnimation());
    }

    /// <summary>
    /// 蜀・′蜃ｺ迴ｾ縺吶ｋ髫帙・繝昴ャ繝励う繝ｳ・亥ｼｾ繧繧医≧縺ｪ・峨い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧定｡後≧繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator SpawnAnimation()
    {
        Vector3 targetScale = transform.localScale;
        transform.localScale = Vector3.zero;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 繧､繝ｼ繧ｺ繧｢繧ｦ繝医ヰ繝・け縺ｮ繧医≧縺ｪ蠑ｾ繧蜍輔″
            float t = elapsed / duration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) + Mathf.Sin(t * Mathf.PI) * 0.2f;
            transform.localScale = targetScale * scale;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    /// <summary>
    /// 豈弱ヵ繝ｬ繝ｼ繝蜻ｼ縺ｰ繧後∵欠螳壹＆繧後◆繧ｰ繝ｩ繝・・繧ｷ繝ｧ繝ｳ縺ｫ蠕薙▲縺ｦ蜀・・濶ｲ繧貞ｾ舌・↓螟牙喧縺輔○繧九・    /// </summary>
    private void Update()
    {
        if (isDying) return;

        // 豈弱ヵ繝ｬ繝ｼ繝濶ｲ繧偵げ繝ｩ繝・・繧ｷ繝ｧ繝ｳ縺ｫ蠕薙▲縺ｦ螟牙喧縺輔○繧・        if (spriteRenderer != null && colorGradient != null)
        {
            // 譎る俣邨碁℃縺ｧ 0.0 縲・1.0 縺ｮ髢薙ｒ陦後▲縺溘ｊ譚･縺溘ｊ・・ingPong・峨＆縺帙ｋ
            float t = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
            spriteRenderer.color = colorGradient.Evaluate(t);
        }
    }

    /// <summary>
    /// 繝槭え繧ｹ縺ｧ繧ｯ繝ｪ繝・け・医∪縺溘・繧ｹ繝槭・縺ｧ繧ｿ繝・・・峨＆繧後◆迸ｬ髢薙↓蜻ｼ縺ｰ繧後ｋ Unity 邨・∩霎ｼ縺ｿ縺ｮ繝｡繧ｽ繝・ラ縲・    /// 繧ｪ繝悶ず繧ｧ繧ｯ繝医↓ Collider (蠖薙◆繧雁愛螳・ 縺御ｻ倥＞縺ｦ縺・ｋ蠢・ｦ√′縺ゅｊ縺ｾ縺吶・    /// </summary>
    private void OnMouseDown()
    {
        // GameManager 縺瑚ｦ九▽縺九ｉ縺ｪ縺・√ご繝ｼ繝縺後☆縺ｧ縺ｫ邨ゆｺ・＠縺ｦ縺・ｋ縲√∪縺溘・豸域ｻ・い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ荳ｭ縺ｪ繧我ｽ輔ｂ縺励↑縺・        if (gameManager == null || !gameManager.isGameActive || isDying) return;
        isDying = true;

        // --- 繧ｿ繝・・謌仙粥譎ゅ・蜃ｦ逅・---

        // 1. 繧ｹ繧ｳ繧｢繧・轤ｹ蜉邂励☆繧・        int addedScore = gameManager.AddScore(1);

        // --- 繧ｿ繝・・髻ｳ・医さ繝ｳ繝懊〒繝斐ャ繝√′荳翫′繧具ｼ峨ｒ魑ｴ繧峨☆ ---
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTapSound(gameManager.CurrentScore);
        }

        // --- 逕ｻ髱｢繧呈昭繧峨＠縺ｦ蜈峨ｉ縺帙ｋ・医き繝・さ縺・＞貍泌・・・---
        if (GameJuiceManager.Instance != null)
        {
            GameJuiceManager.Instance.PlayHitEffect(gameManager.CurrentScore);
        }

        // 2. 谺｡縺ｮ蜀・ｒ逕滓・縺励√◎縺ｮ菴咲ｽｮ繧貞叙蠕励☆繧・        Vector3 nextCirclePos = gameManager.SpawnNextCircle(transform.position);

        // 繧ｳ繝ｩ繧､繝繝ｼ繧堤┌蜉ｹ蛹悶＠縺ｦ縺薙ｌ莉･荳頑款縺帙↑縺・ｈ縺・↓縺吶ｋ
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 4. 辷ｽ蠢ｫ諢溘い繝・・縺ｮ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧帝幕蟋九＠縺ｦ豸医∴繧・        StartCoroutine(DeathAnimation(nextCirclePos));
    }

    /// <summary>
    /// 繧ｿ繝・・・医け繝ｪ繝・け・画・蜉滓凾縲∝・縺梧ｶ域ｻ・☆繧矩圀縺ｮ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ縺ｨ繧ｨ繝輔ぉ繧ｯ繝育函謌舌ｒ陦後≧繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator DeathAnimation(Vector3 nextCirclePos)
    {
        // 辷ｽ蠢ｫ諢溘い繝・・縺ｮ繧ｨ繝輔ぉ繧ｯ繝医ｒ閾ｪ蜍慕函謌撰ｼ・erfect縺ｮ譁・ｭ励□縺第ｮ九☆・・        CreatePerfectText();

        // 閾ｪ霄ｫ縺悟ｰ代＠閹ｨ繧峨ｓ縺ｧ縺九ｉ豸医∴繧九い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.5f; // 1.5蛟阪↓閹ｨ繧峨・

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

        // 閾ｪ霄ｫ縺ｮ陦ｨ遉ｺ繧呈ｶ医☆
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 蜀・′豸医∴縺溽峩蠕後↓繧ｨ繝輔ぉ繧ｯ繝医ｒ逕滓・
        if (clickEffectPrefab != null)
        {
            // 谺｡縺ｮ蜀・↓蜷代°縺｣縺ｦ繧ｨ繝輔ぉ繧ｯ繝医ｒ蜷代￠繧九◆繧√・蝗櫁ｻ｢繧定ｨ育ｮ・            Vector3 direction = nextCirclePos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // 2D・・霆ｸ蝗櫁ｻ｢・峨〒譁ｹ蜷代ｒ蜷医ｏ縺帙ｋ縲ょｿ・ｦ√↓蠢懊§縺ｦ隗貞ｺｦ縺ｮ繧ｪ繝輔そ繝・ヨ・・90縺ｪ縺ｩ・峨ｒ隱ｿ謨ｴ縺吶ｋ
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject effect = Instantiate(clickEffectPrefab, transform.position, rotation);
            
            // 蜀・・繧ｵ繧､繧ｺ縺ｫ蜷医ｏ縺帙※繧ｨ繝輔ぉ繧ｯ繝医・螟ｧ縺阪＆繧定ｪｿ謨ｴ・亥ｰ代＠蟆上＆繧√↓縺吶ｋ縺ｪ繧・.8f縺ｪ縺ｩ・・            effect.transform.localScale = transform.localScale * 0.8f;
            
            ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                // 蜀咲函騾溷ｺｦ繧剃ｸ翫￡縺ｦ縲∬｡ｨ遉ｺ譎る俣繧堤洒縺擾ｼ医せ繝斐・繝・ぅ縺ｫ・峨☆繧・                var main = ps.main;
                main.simulationSpeed = 2.0f; // 2蛟埼・                
                var r = ps.GetComponent<ParticleSystemRenderer>();
                if (r != null)
                {
                    r.sortingLayerName = "Default";
                    r.sortingOrder = 100;
                }
            }
            
            Destroy(effect, 1.5f);
        }

        // 繧ｨ繝輔ぉ繧ｯ繝医・繧ｳ繝ｫ繝ｼ繝√Φ・域怙螟ｧ0.5遘抵ｼ峨′譛蠕後∪縺ｧ蜀咲函縺輔ｌ繧九ｈ縺・↓蠕・ｩ溘☆繧・        yield return new WaitForSeconds(0.6f);

        Destroy(gameObject);
    }

    /// <summary>
    /// 蠎・′縺｣縺ｦ豸医∴繧九Μ繝ｳ繧ｰ・郁｡晄茶豕｢・峨お繝輔ぉ繧ｯ繝医ｒ繝励Ο繧ｰ繝ｩ繝縺ｧ逕滓・縺吶ｋ
    /// </summary>
    private void CreateRippleEffect()
    {
        GameObject rippleObj = new GameObject("Ripple");
        rippleObj.transform.position = transform.position;
        LineRenderer lr = rippleObj.AddComponent<LineRenderer>();
        
        int segments = 36;
        lr.positionCount = segments + 1;
        lr.useWorldSpace = false;
        lr.startWidth = 0.3f; // 蟆代＠螟ｪ繧・        lr.endWidth = 0f;
        
        // 2D逕ｨ縺ｮ讓呎ｺ悶・繝・Μ繧｢繝ｫ繧剃ｽｿ逕ｨ
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

    /// <summary>
    /// 蠎・′縺｣縺ｦ豸医∴繧九Μ繝ｳ繧ｰ・域ｳ｢邏具ｼ峨お繝輔ぉ繧ｯ繝医・繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧定｡後≧繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator AnimateRipple(GameObject obj, LineRenderer lr)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 2.5f; // 螟ｧ縺阪￥蠎・′繧・
        while (elapsed < duration)
        {
            if (obj == null) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 繧､繝ｼ繧ｺ繧｢繧ｦ繝茨ｼ亥ｾ舌・↓驕・￥縺ｪ繧具ｼ峨〒蠎・′繧・            float easeOut = 1f - (1f - t) * (1f - t);
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, easeOut);
            
            // 繝輔ぉ繝ｼ繝峨い繧ｦ繝・            Color color = lr.startColor;
            color.a = Mathf.Lerp(1f, 0f, t);
            lr.startColor = color;

            yield return null;
        }
        Destroy(obj);
    }

    /// <summary>
    /// 鬟帙・謨｣繧句ｰ上＆縺ｪ轣ｫ闃ｱ繝代・繝・ぅ繧ｯ繝ｫ繧偵・繝ｭ繧ｰ繝ｩ繝縺ｧ逕滓・縺吶ｋ
    /// </summary>
    private void CreateSparks()
    {
        GameObject sparksObj = new GameObject("Sparks");
        // 逕滓・譎ゅ↓閾ｪ蜍募・逕溘＆繧後※duration螟画峩譎ゅ↓繧ｨ繝ｩ繝ｼ縺悟・繧九・繧帝亟縺舌◆繧√∽ｸ譎ら噪縺ｫ髱槭い繧ｯ繝・ぅ繝悶↓縺吶ｋ
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
        // 荳迸ｬ縺ｧ螟壹ａ縺ｮ繝代・繝・ぅ繧ｯ繝ｫ繧呈叛蜃ｺ
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)20) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // 遶懷ｷｻ繧・ｸｦ縺ｮ繧医≧縺ｫ蝗櫁ｻ｢・医せ繝斐Φ・峨＠縺ｪ縺後ｉ蠎・′繧・        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        // Particle Orbital Velocity curves must all be in the same mode 縺ｮ繧ｨ繝ｩ繝ｼ蝗樣∩縺ｮ縺溘ａ縲・        // X縺ｨY縺ｫ繧ょ酔縺倥Δ繝ｼ繝会ｼ・woConstants・峨・繝繝溘・蛟､繧定ｨｭ螳壹☆繧・        vel.orbitalX = new ParticleSystem.MinMaxCurve(0f, 0f);
        vel.orbitalY = new ParticleSystem.MinMaxCurve(0f, 0f);
        vel.orbitalZ = new ParticleSystem.MinMaxCurve(5f, 15f); // 縺舌ｋ縺舌ｋ蝗槭ｋ
        vel.radial = new ParticleSystem.MinMaxCurve(2f, 5f);    // 螟門・縺ｫ蠎・′繧・
        // 蜈峨・霆瑚ｷ｡・亥ｰｾ・峨ｒ蠑輔°縺帙ｋ
        var trails = ps.trails;
        trails.enabled = true;
        trails.ratio = 1f; // 蜈ｨ縺ｦ縺ｫ霆瑚ｷ｡繧偵▽縺代ｋ
        trails.dieWithParticles = true;
        trails.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, 0f);
        trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, 0.5f), new Color(1f, 1f, 1f, 0f));

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        
        // 繝励Ο繧ｰ繝ｩ繝縺ｧ縲梧弌・医く繝ｩ繧ｭ繝ｩ・峨阪・繝・け繧ｹ繝√Ε繧堤函謌舌＠縺ｦ蜑ｲ繧雁ｽ薙※繧・        Texture2D starTex = CreateStarTexture();
        Material starMat = new Material(Shader.Find("Sprites/Default"));
        starMat.mainTexture = starTex;
        
        renderer.material = starMat;
        renderer.trailMaterial = new Material(Shader.Find("Sprites/Default")); // 霆瑚ｷ｡縺ｯ譎ｮ騾壹・邱・
        // 險ｭ螳壹′邨ゅｏ縺｣縺溘ｉ繧｢繧ｯ繝・ぅ繝悶↓縺励※蜀咲函髢句ｧ・        sparksObj.SetActive(true);

        Destroy(sparksObj, 1f);
    }

    /// <summary>
    /// 繧ｭ繝ｩ繧ｭ繝ｩ・域弌蝙具ｼ峨・繝・け繧ｹ繝√Ε繧偵・繝ｭ繧ｰ繝ｩ繝縺ｧ逕滓・縺吶ｋ
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
                
                // 蜊∝ｭ励・蜈峨・險育ｮ・                float alpha = 1f - Mathf.Clamp01(Mathf.Abs(u * v) * 15f); 
                // 蜀・ｽ｢縺ｫ繝輔ぉ繝ｼ繝峨い繧ｦ繝医＆縺帙※閾ｪ辟ｶ縺ｪ蠖｢縺ｫ
                float dist = Mathf.Sqrt(u * u + v * v);
                alpha *= Mathf.Clamp01(1f - dist);
                
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Max(0f, alpha)));
            }
        }
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 豸医∴繧区凾縺ｫ縲訓erfect!縲阪・譁・ｭ励′鬟帙・蜃ｺ縺・    /// </summary>
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

        // UI繧医ｊ蜑阪↓陦ｨ遉ｺ縺吶ｋ縺溘ａ縺ｮ繝ｬ繧､繝､繝ｼ險ｭ螳・        MeshRenderer mr = textObj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "Default";
            mr.sortingOrder = 50;
        }

        StartCoroutine(AnimatePerfectText(textObj, tm));
    }

    /// <summary>
    /// 縲訓erfect!縲阪・繝・く繧ｹ繝医ｒ荳翫↓繝輔Ρ繝・→豬ｮ縺九○縺ｪ縺後ｉ繝輔ぉ繝ｼ繝峨い繧ｦ繝医＆縺帙ｋ繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    private IEnumerator AnimatePerfectText(GameObject obj, TextMesh tm)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 1.0f; // 蟆代＠荳翫↓繝輔Ρ繝・→豬ｮ縺・        
        Vector3 startScale = Vector3.zero;
        Vector3 peakScale = Vector3.one * 1.2f;

        while (elapsed < duration)
        {
            if (obj == null) break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 繧､繝ｼ繧ｺ繧｢繧ｦ繝医〒荳翫↓遘ｻ蜍・            float easeOut = 1f - (1f - t) * (1f - t);
            obj.transform.position = Vector3.Lerp(startPos, endPos, easeOut);

            // 繝昴ャ繝励い繝・・縺励※豸医∴繧九せ繧ｱ繝ｼ繝ｫ螟牙喧縺ｨ騾乗・蠎ｦ
            if (t < 0.3f)
            {
                // 繝昴ャ繝励う繝ｳ・域僑螟ｧ・・                obj.transform.localScale = Vector3.Lerp(startScale, peakScale, t / 0.3f);
            }
            else
            {
                // 繧・▲縺上ｊ蜈・・繧ｵ繧､繧ｺ縺ｫ謌ｻ繧翫↑縺後ｉ繝輔ぉ繝ｼ繝峨い繧ｦ繝・                float fadeT = (t - 0.3f) / 0.7f;
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