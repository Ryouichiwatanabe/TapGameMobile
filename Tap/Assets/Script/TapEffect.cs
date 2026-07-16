using UnityEngine;

/// <summary>
/// タップした位置でパーティクルをはじかせるエフェクトスクリプト。
/// Particle System コンポーネントと一緒に使います。
/// </summary>
// このスクリプトをアタッチすると、自動的に ParticleSystem も追加される便利な属性
[RequireComponent(typeof(ParticleSystem))]
public class TapEffect : MonoBehaviour
{
    // パーティクルシステムを操作するための変数
    private ParticleSystem ps;

    private void Awake()
    {
        // 自分の GameObject についている ParticleSystem コンポーネントを取得しておく
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        // 生成された瞬間にパーティクルを再生開始
        ps.Play();

        // パーティクルの再生にかかる全体の時間（基本の長さ + バラツキの最大値）を計算
        float duration = ps.main.duration + ps.main.startLifetime.constantMax;
        
        // 再生が確実に終わるタイミングで、自分自身（エフェクト）を削除してメモリを節約する
        Destroy(gameObject, duration);
    }
}
