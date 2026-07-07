using UnityEngine;

/// <summary>
/// プログラムによる波形生成（Procedural Audio）で効果音を鳴らすサウンドマネージャー。
/// 外部の音声ファイル（mp3/wav）に依存せず、レトロでピコピコした気持ちいい音を生み出します。
/// ゲーム会社提出時にも「音声合成を自作している」という技術的なアピールになります。
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource; // 効果音（プロシージャル）用
    private AudioSource bgmSource;   // BGM再生用
    private float sampleRate;
    private System.Random random;

    // --- シンセサイザー用のパラメータ ---
    // 別スレッド（オーディオスレッド）からアクセスされるため、競合を避ける設計
    private double phase;
    private double frequency;
    private double currentVolume;
    private double duration;
    private double timeElapsed;
    
    private enum WaveType { Square, Sine, Noise }
    private WaveType currentWaveType;

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
            // 空間音響の影響を受けないよう完全な2Dサウンドに設定
            audioSource.spatialBlend = 0f;
            
            sampleRate = AudioSettings.outputSampleRate;
            
            // OnAudioFilterRead を常に動かすため、空のAudioClipをセットしてループ再生しておく
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
    /// オーディオ波形を直接生成するUnityのコールバック関数。
    /// サンプリングレートごとにこのメソッドが別スレッドで呼ばれ、配列（data）に波形データを書き込みます。
    /// </summary>
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (timeElapsed >= duration)
        {
            // 音が鳴り終わっている場合は無音を出力してノイズを防ぐ
            for (int i = 0; i < data.Length; i++) data[i] = 0f;
            return;
        }

        double phaseIncrement = frequency / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            // 時間経過によるエンベロープ生成（音の立ち上がりと消え際のプツッというノイズを防ぐ）
            float envelope = 1f;
            if (timeElapsed < 0.01) envelope = (float)(timeElapsed / 0.01); // アタック（徐々に大きく）
            else if (duration - timeElapsed < 0.02) envelope = (float)((duration - timeElapsed) / 0.02); // リリース（徐々に小さく）
            
            float value = 0f;
            
            switch(currentWaveType)
            {
                case WaveType.Square:
                    // 矩形波（ファミコン風のピコッという音）
                    value = (phase % 1.0 < 0.5) ? 1f : -1f;
                    break;
                case WaveType.Sine:
                    // サイン波（丸い音、ポーンという音）
                    value = (float)System.Math.Sin(phase * 2.0 * System.Math.PI);
                    break;
                case WaveType.Noise:
                    // ノイズ（打撃音や爆発音）
                    value = (float)(random.NextDouble() * 2.0 - 1.0);
                    break;
            }

            value *= (float)currentVolume * envelope;

            // 全チャンネル（左右）に同じ音を書き込む（モノラル）
            for (int c = 0; c < channels; c++)
            {
                data[i + c] = value;
            }

            phase = (phase + phaseIncrement) % 1.0;
            timeElapsed += 1.0 / sampleRate;
            
            if (timeElapsed >= duration) break;
        }
    }

    /// <summary>
    /// 指定した周波数と長さで音を鳴らします。
    /// </summary>
    private void PlaySynthesizedSound(double freq, double vol, double dur, WaveType waveType)
    {
        frequency = freq;
        currentVolume = vol;
        duration = dur;
        currentWaveType = waveType;
        phase = 0;
        timeElapsed = 0; // 時間をリセットして音を再生開始
    }

    // ==========================================
    // 外部から呼ばれる公開メソッド群
    // ==========================================

    /// <summary>
    /// タップ成功時の効果音（連続タップでピッチが上がる）
    /// </summary>
    /// <param name="comboCount">現在のコンボ数（0から開始）</param>
    public void PlayTapSound(int comboCount)
    {
        // コンボが続くほど音が高くなる（基本周波数440Hz：ラ の音から開始）
        float baseFreq = 440f;
        // 1コンボごとに半音ずつ上げる。最大30半音（2.5オクターブ：約2489Hz）まで上がるように変更
        double freq = baseFreq * Mathf.Pow(1.05946f, Mathf.Min(comboCount, 30)); 
        PlaySynthesizedSound(freq, 0.15, 0.1, WaveType.Square);
    }

    /// <summary>
    /// カウントダウンの「ピッ」という音
    /// </summary>
    public void PlayCountdownTick()
    {
        PlaySynthesizedSound(660.0, 0.2, 0.05, WaveType.Sine);
    }

    /// <summary>
    /// ゲーム開始の「ポーン！」という音
    /// </summary>
    public void PlayCountdownGo()
    {
        PlaySynthesizedSound(880.0, 0.25, 0.4, WaveType.Sine);
    }

    /// <summary>
    /// ゲームオーバー・ミス時の音
    /// </summary>
    public void PlayMissSound()
    {
        PlaySynthesizedSound(200.0, 0.2, 0.3, WaveType.Noise);
    }

    /// <summary>
    /// BGMを再生します（再生中のBGMと同じ場合はそのまま流し続けます）
    /// </summary>
    public void PlayBGM(AudioClip bgmClip, float volume = 0.5f)
    {
        if (bgmSource == null || bgmClip == null) return;
        
        // すでに同じ曲が流れている場合は最初から再生し直さない
        if (bgmSource.clip == bgmClip && bgmSource.isPlaying) return;

        bgmSource.clip = bgmClip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    /// <summary>
    /// BGMを停止します
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    /// <summary>
    /// BGMをフェードアウトさせながら停止します
    /// </summary>
    public void FadeOutBGM(float duration)
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }
    }

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
