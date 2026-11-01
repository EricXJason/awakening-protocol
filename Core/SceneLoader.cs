using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ============================================================================
/// 檔案名稱: SceneLoader.cs
/// 所屬模組: Core
/// 責任描述: 全域常駐非同步場景切換管理器。提供 LoadSceneByName、LoadNextScene
///           與 ReloadCurrentScene 服務，包含防重複加載鎖定。
/// 架構分層: Core / Manager
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景根節點下的空物件，命名為「[SceneLoader]」。
/// 2. 生命週期: 腳本內建 DontDestroyOnLoad，保持全域常駐。
/// ============================================================================
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    public static SceneLoader Instance => instance;

    private bool isLoading = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
#if UNITY_EDITOR
            Debug.LogError("[SceneLoader] Target scene name is null or empty, aborting load.");
#endif
            return;
        }

        if (isLoading) return;
        StartCoroutine(LoadAsyncRoutine(sceneName));
    }

    public void LoadNextScene()
    {
        if (isLoading) return;

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadAsyncRoutine(nextIndex));
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("[SceneLoader] No subsequent scene found in Build Settings.");
#endif
        }
    }

    public void ReloadCurrentScene()
    {
        if (isLoading) return;
        StartCoroutine(LoadAsyncRoutine(SceneManager.GetActiveScene().buildIndex));
    }

    private IEnumerator LoadAsyncRoutine(string name)
    {
        isLoading = true;
#if UNITY_EDITOR
        Debug.Log($"[SceneLoader] Starting async load for scene: {name}");
#endif
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(name);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
        isLoading = false;
    }

    private IEnumerator LoadAsyncRoutine(int index)
    {
        isLoading = true;
#if UNITY_EDITOR
        Debug.Log($"[SceneLoader] Starting async load for scene index: {index}");
#endif
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(index);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
        isLoading = false;
    }
}
