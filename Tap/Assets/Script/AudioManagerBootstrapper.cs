using UnityEngine;

/// <summary>
/// 繧ｲ繝ｼ繝髢句ｧ区凾縺ｫAudioManager繧定・蜍慕噪縺ｫ逕滓・縺励∝・繧ｷ繝ｼ繝ｳ縺ｧ蛻ｩ逕ｨ蜿ｯ閭ｽ縺ｫ縺吶ｋ繝倥Ν繝代・繧ｯ繝ｩ繧ｹ縲・/// 髢狗匱閠・′蜷・す繝ｼ繝ｳ縺ｫ謇句虚縺ｧ驟咲ｽｮ縺吶ｋ謇矩俣繧堤怐縺阪∪縺吶・/// </summary>
public static class AudioManagerBootstrapper
{
    /// <summary>
    /// 繧ｷ繝ｼ繝ｳ繝ｭ繝ｼ繝牙燕縺ｫAudioManager繧定・蜍慕函謌舌＠縲∝・譛溷喧縺励∪縺吶・    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // 譌｢縺ｫ蟄伜惠縺吶ｋ蝣ｴ蜷医・菴輔ｂ縺励↑縺・        if (AudioManager.Instance != null) return;

        GameObject audioManagerObj = new GameObject("AudioManager");
        
        // 蜉ｹ譫憺浹・・rocedural Audio・臥畑縺ｮAudioSource
        audioManagerObj.AddComponent<AudioSource>();
        
        // BGM逕ｨ縺ｯ OnAudioFilterRead 縺ｮ蠖ｱ髻ｿ繧貞女縺代↑縺・ｈ縺・∝ｭ舌が繝悶ず繧ｧ繧ｯ繝医↓蛻・￠繧・        GameObject bgmObj = new GameObject("BGMPlayer");
        bgmObj.transform.SetParent(audioManagerObj.transform);
        AudioSource bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0f; // 2D繧ｵ繧ｦ繝ｳ繝・        
        audioManagerObj.AddComponent<AudioManager>();
        
        // AudioManager蜀・〒 DontDestroyOnLoad(gameObject) 縺悟他縺ｰ繧後ｋ縺溘ａ遐ｴ譽・＆繧後∪縺帙ｓ
    }
}
