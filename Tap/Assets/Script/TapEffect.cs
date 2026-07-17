using UnityEngine;

/// <summary>
/// 繧ｿ繝・・縺励◆菴咲ｽｮ縺ｧ繝代・繝・ぅ繧ｯ繝ｫ繧偵・縺倥°縺帙ｋ繧ｨ繝輔ぉ繧ｯ繝医せ繧ｯ繝ｪ繝励ヨ縲・/// Particle System 繧ｳ繝ｳ繝昴・繝阪Φ繝医→荳邱偵↓菴ｿ縺・∪縺吶・/// </summary>
// 縺薙・繧ｹ繧ｯ繝ｪ繝励ヨ繧偵い繧ｿ繝・メ縺吶ｋ縺ｨ縲∬・蜍慕噪縺ｫ ParticleSystem 繧りｿｽ蜉縺輔ｌ繧倶ｾｿ蛻ｩ縺ｪ螻樊ｧ
[RequireComponent(typeof(ParticleSystem))]
public class TapEffect : MonoBehaviour
{
    // 繝代・繝・ぅ繧ｯ繝ｫ繧ｷ繧ｹ繝・Β繧呈桃菴懊☆繧九◆繧√・螟画焚
    private ParticleSystem ps;

    private void Awake()
    {
        // 閾ｪ蛻・・ GameObject 縺ｫ縺､縺・※縺・ｋ ParticleSystem 繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ蜿門ｾ励＠縺ｦ縺翫￥
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        // 逕滓・縺輔ｌ縺溽椪髢薙↓繝代・繝・ぅ繧ｯ繝ｫ繧貞・逕滄幕蟋・        ps.Play();

        // 繝代・繝・ぅ繧ｯ繝ｫ縺ｮ蜀咲函縺ｫ縺九°繧句・菴薙・譎る俣・亥渕譛ｬ縺ｮ髟ｷ縺・+ 繝舌Λ繝・く縺ｮ譛螟ｧ蛟､・峨ｒ險育ｮ・        float duration = ps.main.duration + ps.main.startLifetime.constantMax;
        
        // 蜀咲函縺檎｢ｺ螳溘↓邨ゅｏ繧九ち繧､繝溘Φ繧ｰ縺ｧ縲∬・蛻・・霄ｫ・医お繝輔ぉ繧ｯ繝茨ｼ峨ｒ蜑企勁縺励※繝｡繝｢繝ｪ繧堤ｯ邏・☆繧・        Destroy(gameObject, duration);
    }
}
