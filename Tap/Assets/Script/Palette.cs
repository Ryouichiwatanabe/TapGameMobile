using UnityEngine;

/// <summary>
/// ゲーム全体の統一感を出すためのカラーパレット（3色設定）
/// 明度（Brightness）を-20%（0.8付近）に抑え、目に優しく視認性の高い色に統一しています。
/// </summary>
public static class Palette
{
    // 明度を抑えた3つのテーマカラー
    // 1. シアン（ベース・安全）
    public static readonly Color ColorA = Color.HSVToRGB(180f / 360f, 0.7f, 0.8f);
    
    // 2. オレンジ（アクセント・注意）
    public static readonly Color ColorB = Color.HSVToRGB(35f / 360f, 0.75f, 0.8f);
    
    // 3. マゼンタ/パープル（危険・終盤）
    public static readonly Color ColorC = Color.HSVToRGB(300f / 360f, 0.65f, 0.8f);

    /// <summary>
    /// 円（ターゲット）などの色変化に使うグラデーション
    /// </summary>
    public static Gradient GetMainGradient()
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(ColorA, 0.0f), 
                new GradientColorKey(ColorB, 0.5f), 
                new GradientColorKey(ColorC, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f), 
                new GradientAlphaKey(1f, 1f) 
            }
        );
        return g;
    }

    /// <summary>
    /// タイマー用のグラデーション（満タン: ColorA -> 半分: ColorB -> ピンチ: ColorC）
    /// </summary>
    public static Gradient GetTimerGradient()
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(ColorC, 0.0f), 
                new GradientColorKey(ColorB, 0.4f), 
                new GradientColorKey(ColorA, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f), 
                new GradientAlphaKey(1f, 1f) 
            }
        );
        return g;
    }
}
