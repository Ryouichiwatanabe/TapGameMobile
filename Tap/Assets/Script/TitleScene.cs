using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 繧ｿ繧､繝医Ν逕ｻ髱｢・・itleScene・峨・蛻ｶ蠕｡繧定｡後≧繧ｹ繧ｯ繝ｪ繝励ヨ縲・/// 繝ｭ繧ｴ繧偵・繧上・繧上い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ縺輔○縺溘ｊ縲∫判髱｢繧ｿ繝・・縺ｧ繧ｲ繝ｼ繝逕ｻ髱｢縺ｸ驕ｷ遘ｻ縺輔○縺溘ｊ縺励∪縺吶・/// </summary>
public class TitleScene : MonoBehaviour
{
    // --- 螳壽焚 ---
    // 驕ｷ遘ｻ蜈医・繧ｷ繝ｼ繝ｳ蜷阪ｒ螳壽焚縺ｧ螳夂ｾｩ縺励※縺翫￥縺薙→縺ｧ縲∵遠縺｡髢馴＆縺・ｒ髦ｲ縺弱∪縺・    private const string NEXT_SCENE = "Game";

    // --- 繝ｭ繧ｴ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ險ｭ螳・---
    [Header("繧ｿ繧､繝医Ν繝ｭ繧ｴ縺ｮ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ險ｭ螳・)]
    [Tooltip("蜍輔°縺励◆縺・Ο繧ｴ縺ｮ Transform (蠎ｧ讓吶↑縺ｩ縺ｮ諠・ｱ)")]
    [SerializeField] private Transform titleLogo;
    [Tooltip("繝ｭ繧ｴ縺御ｸ贋ｸ九↓蜍輔￥騾溘＆")]
    [SerializeField] private float moveSpeed  = 2f;
    [Tooltip("繝ｭ繧ｴ縺御ｸ贋ｸ九↓蜍輔￥蟷・ｼ亥､ｧ縺阪＆・・)]
    [SerializeField] private float moveAmount = 20f;

    // --- 繝輔ぉ繝ｼ繝芽ｨｭ螳・---
    [Header("繝輔ぉ繝ｼ繝芽ｨｭ螳・)]
    [Tooltip("逕ｻ髱｢繧呈囓縺上☆繧九◆繧√・鮟偵＞逕ｻ蜒・)]
    [SerializeField] private Image  fadeImage;
    [Tooltip("繝輔ぉ繝ｼ繝峨い繧ｦ繝医↓縺九°繧区凾髢難ｼ育ｧ抵ｼ・)]
    [SerializeField] private float  fadeDuration  = 1f;
    [Tooltip("谺｡縺ｫ隱ｭ縺ｿ霎ｼ繧繧ｷ繝ｼ繝ｳ縺ｮ蜷榊燕")]
    [SerializeField] private string nextSceneName = NEXT_SCENE;

    // --- BGM險ｭ螳・---
    [Header("BGM險ｭ螳・)]
    [Tooltip("繧ｿ繧､繝医Ν縺ｧ豬√☆BGM・域悴險ｭ螳壹・蝣ｴ蜷医・閾ｪ蜍輔〒TItleBGM繧定ｪｭ縺ｿ霎ｼ縺ｿ縺ｾ縺呻ｼ・)]
    [SerializeField] private AudioClip bgmClip;

    // --- 蜀・Κ螟画焚 ---
    // 繝ｭ繧ｴ縺ｮ譛蛻昴・菴咲ｽｮ繧定ｦ壹∴縺ｦ縺翫￥縺溘ａ縺ｮ螟画焚
    private Vector3 initialLogoPosition;
    // 繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ荳ｭ縺九←縺・°繧貞愛螳壹☆繧九ヵ繝ｩ繧ｰ・磯｣邯壹ち繝・・髦ｲ豁｢・・    private bool isTransitioning;

    /// <summary>
    /// 繧ｷ繝ｼ繝ｳ縺ｮ蛻晄悄蛹悶りレ譎ｯ險ｭ螳壹。GM蜀咲函縲√Ο繧ｴ縺ｮ蛻晄悄菴咲ｽｮ縺ｮ險倬鹸繧定｡後＞縺ｾ縺吶・    /// </summary>
    private void Start()
    {
        // --- 閭梧勹逕ｻ蜒上・險ｭ螳・---
        BackgroundHelper.SetupBackground("bg_title");

        // --- BGM縺ｮ蜀咲函 ---
        if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Audio/TItleBGM");
        if (AudioManager.Instance != null && bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(bgmClip, 0.4f); // 髻ｳ驥上・0.4f・亥ｰ代＠謗ｧ縺医ａ・・        }

        // 繝ｭ繧ｴ縺瑚ｨｭ螳壹＆繧後※縺・ｌ縺ｰ縲∵怙蛻昴・菴咲ｽｮ繧定ｨ倬鹸縺吶ｋ
        if (titleLogo != null)
            initialLogoPosition = titleLogo.localPosition;

        // 繝輔ぉ繝ｼ繝臥畑縺ｮ鮟偵＞逕ｻ蜒上ｒ譛蛻昴↓騾乗・縺ｫ縺励※縺翫￥
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0f, 0f, 0f, 0f); // 譛蠕後・ 0f 縺碁乗・蠎ｦ (Alpha)
        }
    }

    /// <summary>
    /// 豈弱ヵ繝ｬ繝ｼ繝蜻ｼ縺ｰ繧後√Ο繧ｴ縺ｮ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧・ち繝・・縺ｫ繧医ｋ繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ縺ｮ蛻､螳壹ｒ陦後＞縺ｾ縺吶・    /// </summary>
    private void Update()
    {
        // --- 繝ｭ繧ｴ縺ｮ縺ｵ繧上・繧上い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ ---
        // 縺ｾ縺繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ荳ｭ縺ｧ縺ｪ縺代ｌ縺ｰ縲√Ο繧ｴ繧剃ｸ贋ｸ九↓蜍輔°縺・        if (titleLogo != null && !isTransitioning)
        {
            // 譎る俣 (Time.time) 縺ｨ Sin (繧ｵ繧､繝ｳ豕｢) 繧剃ｽｿ縺｣縺ｦ縲∵ｻ代ｉ縺九↑荳贋ｸ矩°蜍輔・ Y 蠎ｧ讓吶ｒ險育ｮ励☆繧・            float newY = initialLogoPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
            titleLogo.localPosition = new Vector3(initialLogoPosition.x, newY, initialLogoPosition.z);
        }

        // --- 繧ｷ繝ｼ繝ｳ驕ｷ遘ｻ蜃ｦ逅・---
        // 繝槭え繧ｹ縺ｮ蟾ｦ繧ｯ繝ｪ繝・け・医せ繝槭・縺ｮ繧ｿ繝・・・峨′縺輔ｌ縺溘ｉ縲√ご繝ｼ繝逕ｻ髱｢縺ｸ驕ｷ遘ｻ縺吶ｋ
        if (Input.GetMouseButtonDown(0) && !isTransitioning)
        {
            // 繧ｳ繝ｫ繝ｼ繝√Φ繧剃ｽｿ縺｣縺ｦ繝輔ぉ繝ｼ繝牙・逅・ｒ髢句ｧ・            StartCoroutine(FadeAndLoadScene());
        }
    }

    /// <summary>
    /// 逕ｻ髱｢繧貞ｾ舌・↓證励￥縺励※縲∵ｬ｡縺ｮ繧ｷ繝ｼ繝ｳ繧定ｪｭ縺ｿ霎ｼ繧繧ｳ繝ｫ繝ｼ繝√Φ
    /// </summary>
    private IEnumerator FadeAndLoadScene()
    {
        // 驕ｷ遘ｻ荳ｭ繝輔Λ繧ｰ繧堤ｫ九※縺ｦ縲ゞpdate縺ｧ縺ｮ蜈･蜉帙ｒ蜿励￠莉倥￠縺ｪ縺上☆繧・        isTransitioning = true;

        if (fadeImage != null)
        {
            float elapsed = 0f;
            // 邨碁℃譎る俣縺・fadeDuration・医ヵ繧ｧ繝ｼ繝画凾髢難ｼ峨↓驕斐☆繧九∪縺ｧ繝ｫ繝ｼ繝励☆繧・            while (elapsed < fadeDuration)
            {
                // 1繝輔Ξ繝ｼ繝縺ｮ邨碁℃譎る俣繧定ｶｳ縺・                elapsed += Time.deltaTime;
                
                // 邨碁℃譎る俣縺ｮ蜑ｲ蜷・(0.0 縲・1.0) 繧定ｨ育ｮ励＠縺ｦ縲∫判蜒上・騾乗・蠎ｦ繧貞ｾ舌・↓荳翫￡繧・(證励￥縺吶ｋ)
                fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(elapsed / fadeDuration));
                
                // 谺｡縺ｮ繝輔Ξ繝ｼ繝縺ｾ縺ｧ蠕・ｩ溘☆繧・                yield return null;
            }
        }

        // 繝輔ぉ繝ｼ繝峨′邨ゅｏ縺｣縺溘ｉ繧ｷ繝ｼ繝ｳ繧定ｪｭ縺ｿ霎ｼ繧
        // 閾ｪ菴懊・ SceneManager 縺後≠繧後・縺昴ｌ繧剃ｽｿ逕ｨ縺励√↑縺代ｌ縺ｰ Unity 讓呎ｺ悶・繧ゅ・繧剃ｽｿ縺・        if (SceneManager.Instance != null)
            SceneManager.Instance.LoadScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}

