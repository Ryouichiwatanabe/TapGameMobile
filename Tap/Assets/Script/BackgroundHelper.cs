using UnityEngine;

public static class BackgroundHelper
{
    /// <summary>
    /// Resources/Images から画像を読み込み、カメラの背後に全画面表示のSpriteRendererとして配置します。
    /// </summary>
    public static void SetupBackground(string imageName)
    {
        // 画像をTexture2Dとして読み込む
        Texture2D tex = Resources.Load<Texture2D>("Images/" + imageName);
        if (tex == null)
        {
            Debug.LogWarning("Background image not found: " + imageName);
            return;
        }

        // Texture2DからSpriteを生成
        Sprite bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        // 背景用のGameObjectを作成
        GameObject bgObj = new GameObject("Background_" + imageName);
        SpriteRenderer sr = bgObj.AddComponent<SpriteRenderer>();
        sr.sprite = bgSprite;

        // 背景が目立ちすぎないように、暗めの色（Tint）を重ねてUIテキストの視認性を上げる
        sr.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        // 全ての一番奥に描画されるようにする
        sr.sortingOrder = -100;

        // メインカメラのサイズに合わせて画像をスケーリングする
        Camera cam = Camera.main;
        if (cam != null && cam.orthographic)
        {
            float screenHeight = cam.orthographicSize * 2f;
            float screenWidth = screenHeight * cam.aspect;
            
            float spriteHeight = sr.sprite.bounds.size.y;
            float spriteWidth = sr.sprite.bounds.size.x;
            
            // 画面全体を覆うようにスケールを計算（アスペクト比を維持しつつ、画面にフィットさせる：Cover）
            float scaleY = screenHeight / spriteHeight;
            float scaleX = screenWidth / spriteWidth;
            float scale = Mathf.Max(scaleX, scaleY);
            
            bgObj.transform.localScale = new Vector3(scale, scale, 1f);
            
            // カメラの奥（グリッドより後ろ）に配置
            bgObj.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 20f);
            
            // 背景がカメラに追従するように子オブジェクトにする
            bgObj.transform.SetParent(cam.transform);
        }
    }
}
