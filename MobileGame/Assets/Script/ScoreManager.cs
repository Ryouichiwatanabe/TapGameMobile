using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// スコアの計算とUI表示を担当するクラス。
/// </summary>
public class ScoreManager : MonoBehaviour
{
    private const string LAST_SCORE_KEY = "LastScore";
    private const string SCORE_PREFIX   = "Score: ";

    [Header("ゲーム中スコアUI")]
    [SerializeField] private TMP_Text scoreTextTMP;
    [SerializeField] private Text     scoreText;

    [Header("最終スコアUI（ゲームオーバー時）")]
    [SerializeField] private TMP_Text finalScoreTextTMP;
    [SerializeField] private Text     finalScoreText;

    private int score;

    public int Score => score;

    public void Initialize()
    {
        score = 0;
        UpdateScoreText();
    }

    public int AddScore(int basePoints)
    {
        int actualPoints = basePoints;
        score += actualPoints;
        
        UpdateScoreText();
        
        // スコアUIを跳ねさせるアニメーション
        StopAllCoroutines();
        StartCoroutine(PopAnimation());

        return actualPoints;
    }

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
                // サイン波で1.0 -> 1.5 -> 1.0に戻る
                float scale = 1f + Mathf.Sin((elapsed / duration) * Mathf.PI) * 0.5f;
                targetTransform.localScale = Vector3.one * scale;
                yield return null;
            }
            targetTransform.localScale = Vector3.one;
        }
    }

    public void SaveAndShowFinalScore()
    {
        PlayerPrefs.SetInt(LAST_SCORE_KEY, score);
        PlayerPrefs.Save();

        string finalText = SCORE_PREFIX + score;
        if (finalScoreText != null)    finalScoreText.text    = finalText;
        if (finalScoreTextTMP != null) finalScoreTextTMP.text = finalText;
    }

    private void UpdateScoreText()
    {
        string text = SCORE_PREFIX + score;
        
        if (scoreText != null)    scoreText.text    = text;
        if (scoreTextTMP != null) scoreTextTMP.text = text;
    }
}
