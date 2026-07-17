using UnityEngine;

/// <summary>
/// 背景画像を読み込み、カメラの後ろに配置して画面全体を覆うようにスケールを調整するヘルパークラス。
/// </summary>
public static class BackgroundHelper
{
    /// <summary>
    /// Resources/Images から画像をSpriteとして読み込み、カメラの背後に全画面表示のSpriteRendererとして配置します。
    /// </summary>
    /// <param name="imageName">読み込む画像ファイル名（拡張子なし）</param>
    public static void SetupBackground(string imageName)
    {
        // 画像をSpriteとして読み込む（InspectorでSprite(2D and UI)として設定されていることを前提）
        Sprite bgSprite = Resources.Load<Sprite>("Images/" + imageName);
        if (bgSprite == null)
        {
            Debug.LogWarning("Background image not found: " + imageName);
            return;
        }

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
            
            // 画面全体を覆うようにスケールを計算（アスペクト比を維持しつつ、画面にフィットさせる）
            float scaleY = screenHeight / spriteHeight;
            float scaleX = screenWidth / spriteWidth;
            float scale = Mathf.Max(scaleX, scaleY);
            
            bgObj.transform.localScale = new Vector3(scale, scale, 1f);
            
            // カメラの奥（Z=20の位置）に配置
            bgObj.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 20f);
            
            // 背景がカメラに追従するように子オブジェクトにする
            bgObj.transform.SetParent(cam.transform);
        }
    }
}
