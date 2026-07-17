using UnityEngine;

/// <summary>
/// 繧ｷ繝ｼ繝ｳ縺ｮ隱ｭ縺ｿ霎ｼ縺ｿ・磯・遘ｻ・峨ｒ邂｡逅・☆繧九せ繧ｯ繝ｪ繝励ヨ縲・/// 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ・・ingleton・峨→縺・≧險ｭ險医ヱ繧ｿ繝ｼ繝ｳ繧剃ｽｿ縺｣縺ｦ縺翫ｊ縲・/// 繧ｲ繝ｼ繝荳ｭ縺ｫ蟶ｸ縺ｫ1縺､縺縺大ｭ伜惠縺励√←縺ｮ繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ縺ｧ繧らｰ｡蜊倥↓蜻ｼ縺ｳ蜃ｺ縺帙ｋ繧医≧縺ｫ縺ｪ縺｣縺ｦ縺・∪縺吶・/// </summary>
public class SceneManager : MonoBehaviour
{
    // 縺ｩ縺薙°繧峨〒繧・SceneManager.Instance 縺ｧ繧｢繧ｯ繧ｻ繧ｹ縺ｧ縺阪ｋ繧医≧縺ｫ縺吶ｋ縺溘ａ縺ｮ螟画焚
    public static SceneManager Instance { get; private set; }

    /// <summary>
    /// 繧ｹ繧ｯ繝ｪ繝励ヨ縺後Ο繝ｼ繝峨＆繧後◆譎ゅ・蛻晄悄蛹門・逅・・    /// 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ縺ｮ險ｭ螳壹ｒ陦後＞縲√す繝ｼ繝ｳ髢薙〒遐ｴ譽・＆繧後↑縺・ｈ縺・↓縺吶ｋ縲・    /// </summary>
    private void Awake()
    {
        // 縺ｾ縺 Instance 縺瑚ｨｭ螳壹＆繧後※縺・↑縺代ｌ縺ｰ縲∬・蛻・・霄ｫ繧堤匳骭ｲ縺吶ｋ
        if (Instance == null)
        {
            Instance = this;
            // 繧ｷ繝ｼ繝ｳ縺悟・繧頑崛繧上▲縺ｦ繧ゅ％縺ｮ GameObject 縺檎ｴ譽・＆繧後↑縺・ｈ縺・↓縺吶ｋ
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 縺吶〒縺ｫ蛻･縺ｮ SceneManager 縺悟ｭ伜惠縺励※縺・ｌ縺ｰ縲・㍾隍・ｒ髦ｲ縺舌◆繧√↓閾ｪ蛻・・霄ｫ繧貞炎髯､縺吶ｋ
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 謖・ｮ壹＠縺溷錐蜑阪・繧ｷ繝ｼ繝ｳ繧定ｪｭ縺ｿ霎ｼ繧
    /// </summary>
    /// <param name="sceneName">隱ｭ縺ｿ霎ｼ縺ｿ縺溘＞繧ｷ繝ｼ繝ｳ縺ｮ蜷榊燕・井ｾ・ "Game"・・/param>
    public void LoadScene(string sceneName)
    {
        // Unity讓呎ｺ悶・SceneManager繧剃ｽｿ縺｣縺ｦ繧ｷ繝ｼ繝ｳ繧偵Ο繝ｼ繝峨☆繧・        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 迴ｾ蝨ｨ髢九＞縺ｦ縺・ｋ繧ｷ繝ｼ繝ｳ繧呈怙蛻昴°繧芽ｪｭ縺ｿ霎ｼ縺ｿ逶ｴ縺呻ｼ医Μ繝医Λ繧､讖溯・縺ｪ縺ｩ縺ｫ菴ｿ縺・ｼ・    /// </summary>
    public void ReloadScene()
    {
        // 迴ｾ蝨ｨ繧｢繧ｯ繝・ぅ繝悶↑繧ｷ繝ｼ繝ｳ縺ｮ蜷榊燕繧貞叙蠕励＠縺ｦ縲√◎繧後ｒ繝ｭ繝ｼ繝峨＠逶ｴ縺・        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
