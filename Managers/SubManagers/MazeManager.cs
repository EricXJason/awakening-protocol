using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: MazeManager.cs
/// 所屬模組: Managers/SubManagers
/// 責任描述: 形式運思期 (Formal Operational Stage) 迷宮動態拓撲載入器。
///           根據玩家當前失敗計數，動態啟動對應之迷宮格局。
/// 架構分層: Managers / Local SubManager
/// 聯動關係:
///   - 依賴: GameManager
///   - 協同: LevelManager_FormalOperationalStage
/// ============================================================================
/// </summary>
public class MazeManager : MonoBehaviour
{
    [Header("Maze Configuration")]
    [SerializeField] private GameObject[] mazes;

    private void Start()
    {
        if (mazes == null || mazes.Length == 0) return;

        int failIndex = GameManager.Instance != null ? GameManager.Instance.FailCount : 0;
        LoadMaze(failIndex);
    }

    public void LoadMaze(int index)
    {
        if (mazes == null || mazes.Length == 0) return;

        int safeIndex = Mathf.Clamp(index, 0, mazes.Length - 1);
        for (int i = 0; i < mazes.Length; i++)
        {
            if (mazes[i] != null)
            {
                mazes[i].SetActive(i == safeIndex);
            }
        }

#if UNITY_EDITOR
        Debug.Log($"[MazeManager] Loaded maze index: {safeIndex} (Current fail count: {index})");
#endif
    }
}
