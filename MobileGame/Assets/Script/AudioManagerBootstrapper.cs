using UnityEngine;

/// <summary>
/// ゲーム開始時にAudioManagerを自動的に生成し、全シーンで利用可能にするヘルパークラス。
/// 開発者が各シーンに手動で配置する手間を省きます。
/// </summary>
public static class AudioManagerBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // 既に存在する場合は何もしない
        if (AudioManager.Instance != null) return;

        GameObject audioManagerObj = new GameObject("AudioManager");
        
        // 効果音（Procedural Audio）用のAudioSource
        audioManagerObj.AddComponent<AudioSource>();
        
        // BGM用は OnAudioFilterRead の影響を受けないよう、子オブジェクトに分ける
        GameObject bgmObj = new GameObject("BGMPlayer");
        bgmObj.transform.SetParent(audioManagerObj.transform);
        AudioSource bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0f; // 2Dサウンド
        
        audioManagerObj.AddComponent<AudioManager>();
        
        // AudioManager内で DontDestroyOnLoad(gameObject) が呼ばれるため破棄されません
    }
}
