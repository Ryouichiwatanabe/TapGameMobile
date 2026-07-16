using UnityEngine;

/// <summary>
/// シーンの読み込み（遷移）を管理するスクリプト。
/// シングルトン（Singleton）という設計パターンを使っており、
/// ゲーム中に常に1つだけ存在し、どのスクリプトからでも簡単に呼び出せるようになっています。
/// </summary>
public class SceneManager : MonoBehaviour
{
    // どこからでも SceneManager.Instance でアクセスできるようにするための変数
    public static SceneManager Instance { get; private set; }

    private void Awake()
    {
        // まだ Instance が設定されていなければ、自分自身を登録する
        if (Instance == null)
        {
            Instance = this;
            // シーンが切り替わってもこの GameObject が破棄されないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでに別の SceneManager が存在していれば、重複を防ぐために自分自身を削除する
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 指定した名前のシーンを読み込む
    /// </summary>
    /// <param name="sceneName">読み込みたいシーンの名前（例: "Game"）</param>
    public void LoadScene(string sceneName)
    {
        // Unity標準のSceneManagerを使ってシーンをロードする
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 現在開いているシーンを最初から読み込み直す（リトライ機能などに使う）
    /// </summary>
    public void ReloadScene()
    {
        // 現在アクティブなシーンの名前を取得して、それをロードし直す
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
