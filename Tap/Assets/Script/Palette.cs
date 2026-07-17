using UnityEngine;

/// <summary>
/// 繧ｲ繝ｼ繝蜈ｨ菴薙・邨ｱ荳諢溘ｒ蜃ｺ縺吶◆繧√・繧ｫ繝ｩ繝ｼ繝代Ξ繝・ヨ・・濶ｲ險ｭ螳夲ｼ・/// 譏主ｺｦ・・rightness・峨ｒ-20%・・.8莉倩ｿ托ｼ峨↓謚代∴縲∫岼縺ｫ蜆ｪ縺励￥隕冶ｪ肴ｧ縺ｮ鬮倥＞濶ｲ縺ｫ邨ｱ荳縺励※縺・∪縺吶・/// </summary>
public static class Palette
{
    // 譏主ｺｦ繧呈椛縺医◆3縺､縺ｮ繝・・繝槭き繝ｩ繝ｼ
    // 1. 繧ｷ繧｢繝ｳ・医・繝ｼ繧ｹ繝ｻ螳牙・・・    public static readonly Color ColorA = Color.HSVToRGB(180f / 360f, 0.7f, 0.8f);
    
    // 2. 繧ｪ繝ｬ繝ｳ繧ｸ・医い繧ｯ繧ｻ繝ｳ繝医・豕ｨ諢擾ｼ・    public static readonly Color ColorB = Color.HSVToRGB(35f / 360f, 0.75f, 0.8f);
    
    // 3. 繝槭ぞ繝ｳ繧ｿ/繝代・繝励Ν・亥些髯ｺ繝ｻ邨ら乢・・    public static readonly Color ColorC = Color.HSVToRGB(300f / 360f, 0.65f, 0.8f);

    /// <summary>
    /// 蜀・ｼ医ち繝ｼ繧ｲ繝・ヨ・峨↑縺ｩ縺ｮ濶ｲ螟牙喧縺ｫ菴ｿ縺・げ繝ｩ繝・・繧ｷ繝ｧ繝ｳ
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
    /// 繧ｿ繧､繝槭・逕ｨ縺ｮ繧ｰ繝ｩ繝・・繧ｷ繝ｧ繝ｳ・域ｺ繧ｿ繝ｳ: ColorA -> 蜊雁・: ColorB -> 繝斐Φ繝・ ColorC・・    /// </summary>
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
