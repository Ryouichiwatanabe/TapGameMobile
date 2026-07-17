using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 繧ｹ繧ｳ繧｢縺ｮ險育ｮ励→UI陦ｨ遉ｺ繧呈球蠖薙☆繧九け繝ｩ繧ｹ縲・/// </summary>
public class ScoreManager : MonoBehaviour
{
    private const string LAST_SCORE_KEY = "LastScore";
    private const string SCORE_PREFIX   = "Score: ";

    [Header("繧ｲ繝ｼ繝荳ｭ繧ｹ繧ｳ繧｢UI")]
    [SerializeField] private TMP_Text scoreTextTMP;
    [SerializeField] private Text     scoreText;

    [Header("譛邨ゅせ繧ｳ繧｢UI・医ご繝ｼ繝繧ｪ繝ｼ繝舌・譎ゑｼ・)]
    [SerializeField] private TMP_Text finalScoreTextTMP;
    [SerializeField] private Text     finalScoreText;

    // 迴ｾ蝨ｨ縺ｮ繧ｹ繧ｳ繧｢繧剃ｿ晄戟縺吶ｋ螟画焚
    private int score;

    // 螟夜Κ縺九ｉ迴ｾ蝨ｨ縺ｮ繧ｹ繧ｳ繧｢繧貞叙蠕励☆繧九◆繧√・繝励Ο繝代ユ繧｣
    public int Score => score;

    /// <summary>
    /// 繧ｲ繝ｼ繝髢句ｧ区凾縺ｫ繧ｹ繧ｳ繧｢繧・縺ｫ繝ｪ繧ｻ繝・ヨ縺励∬｡ｨ遉ｺ繧呈峩譁ｰ縺吶ｋ
    /// </summary>
    public void Initialize()
    {
        score = 0;
        UpdateScoreText();
    }

    /// <summary>
    /// 繧ｹ繧ｳ繧｢繧貞刈邂励＠縲ゞI繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧貞・逕溘☆繧・    /// </summary>
    /// <param name="basePoints">蜉邂励☆繧句渕譛ｬ繧ｹ繧ｳ繧｢</param>
    /// <returns>螳滄圀縺ｫ蜉邂励＆繧後◆繧ｹ繧ｳ繧｢</returns>
    public int AddScore(int basePoints)
    {
        // 螳滄圀縺ｮ蜉邂励せ繧ｳ繧｢・亥ｰ・擂逧・↓繝懊・繝翫せ縺ｪ縺ｩ縺ｮ險育ｮ励ｒ蜈･繧後ｋ縺溘ａ縺ｮ螟画焚・・        int actualPoints = basePoints;
        score += actualPoints;
        
        // 繧ｹ繧ｳ繧｢陦ｨ遉ｺ繧呈峩譁ｰ
        UpdateScoreText();
        
        // 繧ｹ繧ｳ繧｢UI繧定ｷｳ縺ｭ縺輔○繧九い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧貞・逕・        StopAllCoroutines();
        StartCoroutine(PopAnimation());

        return actualPoints;
    }

    /// <summary>
    /// 繧ｹ繧ｳ繧｢繝・く繧ｹ繝医′霍ｳ縺ｭ繧九ｈ縺・↑繝昴ャ繝励い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧定｡後≧繧ｳ繝ｫ繝ｼ繝√Φ
    /// </summary>
    private IEnumerator PopAnimation()
    {
        Transform targetTransform = null;
        if (scoreTextTMP != null) targetTransform = scoreTextTMP.transform;
        else if (scoreText != null) targetTransform = scoreText.transform;

        if (targetTransform != null)
        {
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // 繧ｵ繧､繝ｳ豕｢縺ｧ1.0 -> 1.5 -> 1.0縺ｫ謌ｻ繧・                float scale = 1f + Mathf.Sin((elapsed / duration) * Mathf.PI) * 0.5f;
                targetTransform.localScale = Vector3.one * scale;
                yield return null;
            }
            targetTransform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 迴ｾ蝨ｨ縺ｮ繧ｹ繧ｳ繧｢繧剃ｿ晏ｭ倥＠縲√ご繝ｼ繝繧ｪ繝ｼ繝舌・逕ｻ髱｢逕ｨ縺ｮ譛邨ゅせ繧ｳ繧｢繝・く繧ｹ繝医↓蜿肴丐縺吶ｋ
    /// </summary>
    public void SaveAndShowFinalScore()
    {
        // PlayerPrefs繧剃ｽｿ縺｣縺ｦ繝ｭ繝ｼ繧ｫ繝ｫ縺ｫ繧ｹ繧ｳ繧｢繧剃ｿ晏ｭ・        PlayerPrefs.SetInt(LAST_SCORE_KEY, score);
        PlayerPrefs.Save();

        // 譛邨ゅせ繧ｳ繧｢逕ｨ縺ｮUI縺ｫ繝・く繧ｹ繝医ｒ繧ｻ繝・ヨ
        string finalText = SCORE_PREFIX + score;
        if (finalScoreText != null)    finalScoreText.text    = finalText;
        if (finalScoreTextTMP != null) finalScoreTextTMP.text = finalText;
    }

    /// <summary>
    /// 逕ｻ髱｢荳翫・繧ｹ繧ｳ繧｢繝・く繧ｹ繝医ｒ譛譁ｰ縺ｮ蛟､縺ｫ譖ｴ譁ｰ縺吶ｋ
    /// </summary>
    private void UpdateScoreText()
    {
        string text = SCORE_PREFIX + score;
        
        // TextMeshPro縺ｾ縺溘・讓呎ｺ傍ext繧ｳ繝ｳ繝昴・繝阪Φ繝医′繧｢繧ｿ繝・メ縺輔ｌ縺ｦ縺・ｌ縺ｰ譖ｴ譁ｰ
        if (scoreText != null)    scoreText.text    = text;
        if (scoreTextTMP != null) scoreTextTMP.text = text;
    }
}
