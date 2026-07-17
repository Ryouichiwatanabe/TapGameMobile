using UnityEngine;

/// <summary>
/// 繝励Ο繧ｰ繝ｩ繝縺ｫ繧医ｋ豕｢蠖｢逕滓・・・rocedural Audio・峨〒蜉ｹ譫憺浹繧帝ｳｴ繧峨☆繧ｵ繧ｦ繝ｳ繝峨・繝阪・繧ｸ繝｣繝ｼ縲・/// 螟夜Κ縺ｮ髻ｳ螢ｰ繝輔ぃ繧､繝ｫ・・p3/wav・峨↓萓晏ｭ倥○縺壹√Ξ繝医Ο縺ｧ繝斐さ繝斐さ縺励◆豌玲戟縺｡縺・＞髻ｳ繧堤函縺ｿ蜃ｺ縺励∪縺吶・/// 繧ｲ繝ｼ繝莨夂､ｾ謠仙・譎ゅ↓繧ゅ碁浹螢ｰ蜷域・繧定・菴懊＠縺ｦ縺・ｋ縲阪→縺・≧謚陦鍋噪縺ｪ繧｢繝斐・繝ｫ縺ｫ縺ｪ繧翫∪縺吶・/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource; // 蜉ｹ譫憺浹・医・繝ｭ繧ｷ繝ｼ繧ｸ繝｣繝ｫ・臥畑
    private AudioSource bgmSource;   // BGM蜀咲函逕ｨ
    private float sampleRate;
    private System.Random random;

    // --- 繧ｷ繝ｳ繧ｻ繧ｵ繧､繧ｶ繝ｼ逕ｨ縺ｮ繝代Λ繝｡繝ｼ繧ｿ ---
    // 蛻･繧ｹ繝ｬ繝・ラ・医が繝ｼ繝・ぅ繧ｪ繧ｹ繝ｬ繝・ラ・峨°繧峨い繧ｯ繧ｻ繧ｹ縺輔ｌ繧九◆繧√∫ｫｶ蜷医ｒ驕ｿ縺代ｋ險ｭ險・    private double phase;
    private double frequency;
    private double currentVolume;
    private double duration;
    private double timeElapsed;
    
    private enum WaveType { Square, Sine, Noise }
    private WaveType currentWaveType;

    /// <summary>
    /// 繧ｷ繝ｼ繝ｳ髢句ｧ区凾縺ｫ蜻ｼ縺ｳ蜃ｺ縺輔ｌ縲√す繝ｳ繧ｰ繝ｫ繝医Φ縺ｮ蛻晄悄蛹悶→AudioSource縺ｮ險ｭ螳壹ｒ陦後＞縺ｾ縺吶・    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            random = new System.Random();
            
            audioSource = GetComponent<AudioSource>();
            
            Transform bgmPlayer = transform.Find("BGMPlayer");
            if (bgmPlayer != null)
            {
                bgmSource = bgmPlayer.GetComponent<AudioSource>();
            }
            else
            {
                GameObject bgmObj = new GameObject("BGMPlayer");
                bgmObj.transform.SetParent(transform);
                bgmSource = bgmObj.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.spatialBlend = 0f;
            }

            audioSource.playOnAwake = false;
            // 遨ｺ髢馴浹髻ｿ縺ｮ蠖ｱ髻ｿ繧貞女縺代↑縺・ｈ縺・ｮ悟・縺ｪ2D繧ｵ繧ｦ繝ｳ繝峨↓險ｭ螳・            audioSource.spatialBlend = 0f;
            
            sampleRate = AudioSettings.outputSampleRate;
            
            // OnAudioFilterRead 繧貞ｸｸ縺ｫ蜍輔°縺吶◆繧√∫ｩｺ縺ｮAudioClip繧偵そ繝・ヨ縺励※繝ｫ繝ｼ繝怜・逕溘＠縺ｦ縺翫￥
            audioSource.clip = AudioClip.Create("Dummy", 1, 1, (int)sampleRate, false);
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 繧ｪ繝ｼ繝・ぅ繧ｪ豕｢蠖｢繧堤峩謗･逕滓・縺吶ｋUnity縺ｮ繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ髢｢謨ｰ縲・    /// 繧ｵ繝ｳ繝励Μ繝ｳ繧ｰ繝ｬ繝ｼ繝医＃縺ｨ縺ｫ縺薙・繝｡繧ｽ繝・ラ縺悟挨繧ｹ繝ｬ繝・ラ縺ｧ蜻ｼ縺ｰ繧後・・蛻暦ｼ・ata・峨↓豕｢蠖｢繝・・繧ｿ繧呈嶌縺崎ｾｼ縺ｿ縺ｾ縺吶・    /// </summary>
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (timeElapsed >= duration)
        {
            // 髻ｳ縺碁ｳｴ繧顔ｵゅｏ縺｣縺ｦ縺・ｋ蝣ｴ蜷医・辟｡髻ｳ繧貞・蜉帙＠縺ｦ繝弱う繧ｺ繧帝亟縺・            for (int i = 0; i < data.Length; i++) data[i] = 0f;
            return;
        }

        double phaseIncrement = frequency / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            // 譎る俣邨碁℃縺ｫ繧医ｋ繧ｨ繝ｳ繝吶Ο繝ｼ繝礼函謌撰ｼ磯浹縺ｮ遶九■荳翫′繧翫→豸医∴髫帙・繝励ヤ繝・→縺・≧繝弱う繧ｺ繧帝亟縺撰ｼ・            float envelope = 1f;
            if (timeElapsed < 0.01) envelope = (float)(timeElapsed / 0.01); // 繧｢繧ｿ繝・け・亥ｾ舌・↓螟ｧ縺阪￥・・            else if (duration - timeElapsed < 0.02) envelope = (float)((duration - timeElapsed) / 0.02); // 繝ｪ繝ｪ繝ｼ繧ｹ・亥ｾ舌・↓蟆上＆縺擾ｼ・            
            float value = 0f;
            
            switch(currentWaveType)
            {
                case WaveType.Square:
                    // 遏ｩ蠖｢豕｢・医ヵ繧｡繝溘さ繝ｳ鬚ｨ縺ｮ繝斐さ繝・→縺・≧髻ｳ・・                    value = (phase % 1.0 < 0.5) ? 1f : -1f;
                    break;
                case WaveType.Sine:
                    // 繧ｵ繧､繝ｳ豕｢・井ｸｸ縺・浹縲√・繝ｼ繝ｳ縺ｨ縺・≧髻ｳ・・                    value = (float)System.Math.Sin(phase * 2.0 * System.Math.PI);
                    break;
                case WaveType.Noise:
                    // 繝弱う繧ｺ・域遠謦・浹繧・・逋ｺ髻ｳ・・                    value = (float)(random.NextDouble() * 2.0 - 1.0);
                    break;
            }

            value *= (float)currentVolume * envelope;

            // 蜈ｨ繝√Ε繝ｳ繝阪Ν・亥ｷｦ蜿ｳ・峨↓蜷後§髻ｳ繧呈嶌縺崎ｾｼ繧・医Δ繝弱Λ繝ｫ・・            for (int c = 0; c < channels; c++)
            {
                data[i + c] = value;
            }

            phase = (phase + phaseIncrement) % 1.0;
            timeElapsed += 1.0 / sampleRate;
            
            if (timeElapsed >= duration) break;
        }
    }

    /// <summary>
    /// 謖・ｮ壹＠縺溷捉豕｢謨ｰ縺ｨ髟ｷ縺輔〒髻ｳ繧帝ｳｴ繧峨＠縺ｾ縺吶・    /// </summary>
    private void PlaySynthesizedSound(double freq, double vol, double dur, WaveType waveType)
    {
        frequency = freq;
        currentVolume = vol;
        duration = dur;
        currentWaveType = waveType;
        phase = 0;
        timeElapsed = 0; // 譎る俣繧偵Μ繧ｻ繝・ヨ縺励※髻ｳ繧貞・逕滄幕蟋・    }

    // ==========================================
    // 螟夜Κ縺九ｉ蜻ｼ縺ｰ繧後ｋ蜈ｬ髢九Γ繧ｽ繝・ラ鄒､
    // ==========================================

    /// <summary>
    /// 繧ｿ繝・・謌仙粥譎ゅ・蜉ｹ譫憺浹・磯｣邯壹ち繝・・縺ｧ繝斐ャ繝√′荳翫′繧具ｼ・    /// </summary>
    /// <param name="comboCount">迴ｾ蝨ｨ縺ｮ繧ｳ繝ｳ繝懈焚・・縺九ｉ髢句ｧ具ｼ・/param>
    public void PlayTapSound(int comboCount)
    {
        // 繧ｳ繝ｳ繝懊′邯壹￥縺ｻ縺ｩ髻ｳ縺碁ｫ倥￥縺ｪ繧具ｼ亥渕譛ｬ蜻ｨ豕｢謨ｰ440Hz・壹Λ 縺ｮ髻ｳ縺九ｉ髢句ｧ具ｼ・        float baseFreq = 440f;
        // 1繧ｳ繝ｳ繝懊＃縺ｨ縺ｫ蜊企浹縺壹▽荳翫￡繧九よ怙螟ｧ30蜊企浹・・.5繧ｪ繧ｯ繧ｿ繝ｼ繝厄ｼ夂ｴ・489Hz・峨∪縺ｧ荳翫′繧九ｈ縺・↓螟画峩
        double freq = baseFreq * Mathf.Pow(1.05946f, Mathf.Min(comboCount, 30)); 
        PlaySynthesizedSound(freq, 0.15, 0.1, WaveType.Square);
    }

    /// <summary>
    /// 繧ｫ繧ｦ繝ｳ繝医ム繧ｦ繝ｳ縺ｮ縲後ヴ繝・阪→縺・≧髻ｳ
    /// </summary>
    public void PlayCountdownTick()
    {
        PlaySynthesizedSound(660.0, 0.2, 0.05, WaveType.Sine);
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝髢句ｧ九・縲後・繝ｼ繝ｳ・√阪→縺・≧髻ｳ
    /// </summary>
    public void PlayCountdownGo()
    {
        PlaySynthesizedSound(880.0, 0.25, 0.4, WaveType.Sine);
    }

    /// <summary>
    /// 繧ｲ繝ｼ繝繧ｪ繝ｼ繝舌・繝ｻ繝溘せ譎ゅ・髻ｳ
    /// </summary>
    public void PlayMissSound()
    {
        PlaySynthesizedSound(200.0, 0.2, 0.3, WaveType.Noise);
    }

    /// <summary>
    /// BGM繧貞・逕溘＠縺ｾ縺呻ｼ亥・逕滉ｸｭ縺ｮBGM縺ｨ蜷後§蝣ｴ蜷医・縺昴・縺ｾ縺ｾ豬√＠邯壹￠縺ｾ縺呻ｼ・    /// </summary>
    public void PlayBGM(AudioClip bgmClip, float volume = 0.5f)
    {
        if (bgmSource == null || bgmClip == null) return;
        
        // 縺吶〒縺ｫ蜷後§譖ｲ縺梧ｵ√ｌ縺ｦ縺・ｋ蝣ｴ蜷医・譛蛻昴°繧牙・逕溘＠逶ｴ縺輔↑縺・        if (bgmSource.clip == bgmClip && bgmSource.isPlaying) return;

        bgmSource.clip = bgmClip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM繧貞●豁｢縺励∪縺・    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    /// <summary>
    /// BGM繧偵ヵ繧ｧ繝ｼ繝峨い繧ｦ繝医＆縺帙↑縺後ｉ蛛懈ｭ｢縺励∪縺・    /// </summary>
    public void FadeOutBGM(float duration)
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }
    }

    /// <summary>
    /// BGM縺ｮ繝輔ぉ繝ｼ繝峨い繧ｦ繝亥・逅・ｒ陦後≧繧ｳ繝ｫ繝ｼ繝√Φ縲・    /// </summary>
    /// <param name="duration">繝輔ぉ繝ｼ繝峨い繧ｦ繝医↓縺九°繧区凾髢難ｼ育ｧ抵ｼ・/param>
    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVol = bgmSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += UnityEngine.Time.unscaledDeltaTime;
            bgmSource.volume = UnityEngine.Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = startVol;
    }
}
