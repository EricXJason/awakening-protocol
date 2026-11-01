using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: LevelManager_FormalOperationalStage.cs
/// 所屬模組: Managers
/// 責任描述: Stage 4: 形式運思期 (Formal Operational Stage) 迷宮多重分支與雙結局判定。
///           玩家需在動態迷宮中透過邏輯推導選擇正確出口門扉。
///           作為場景中介者 (Mediator)，監聽 MazeDoorController 事件，不使用單例。
/// 架構分層: Managers / Implementation
/// 聯動關係:
///   - 繼承: BaseLevelManager
///   - 依賴: MazeManager, MazeDoorController, SceneLoader
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景中的「[LevelManager]」。
/// ============================================================================
/// </summary>
public class LevelManager_FormalOperationalStage : BaseLevelManager
{
    [Header("Ending Branch Scenes")]
    [SerializeField] private string endingLimboSceneName = "07_Ending_Limbo";
    [SerializeField] private string endingCitySceneName = "07_Ending_City";
    [SerializeField] private int failCountThresholdForLimbo = 4;
    [SerializeField] private int maxFailsAllowed = 8;

    [Header("Maze References")]
    [SerializeField] private MazeDoorController[] mazeDoors;

    protected override void Awake()
    {
        base.Awake();
        stageIndex = 4;
        stageName = "FormalOperationalStage";
    }

    protected override void Start()
    {
        base.Start();

        if (mazeDoors == null || mazeDoors.Length == 0)
        {
            mazeDoors = FindObjectsOfType<MazeDoorController>();
        }

        if (mazeDoors != null)
        {
            foreach (var door in mazeDoors)
            {
                if (door != null) door.OnDoorSelected += HandleDoorSelected;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (mazeDoors != null)
        {
            foreach (var door in mazeDoors)
            {
                if (door != null) door.OnDoorSelected -= HandleDoorSelected;
            }
        }
    }

    /// <summary>
    /// 監聽迷宮出口門扉選擇事件
    /// </summary>
    /// <param name="isCorrectDoor">是否為唯一真實生路</param>
    public void HandleDoorSelected(bool isCorrectDoor)
    {
        if (isCorrectDoor)
        {
            OnMazeSolved();
        }
        else
        {
            OnDoorFailed();
        }
    }

    private void OnMazeSolved()
    {
        IsCompleted = true;

        if (stageSequence != null)
        {
            stageSequence.TriggerEndingProcess();
        }

        int failCount = GameManager.Instance != null ? GameManager.Instance.FailCount : 0;
        string targetScene = failCount >= failCountThresholdForLimbo ? endingLimboSceneName : endingCitySceneName;

#if UNITY_EDITOR
        Debug.Log($"[FormalStage] Maze solved successfully! Total fail count: {failCount}, Target ending scene: {targetScene}");
#endif

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneByName(targetScene);
        }
    }

    private void OnDoorFailed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddFailCount();
            int currentFail = GameManager.Instance.FailCount;

            if (currentFail >= maxFailsAllowed)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[FormalStage] Maximum failures reached, forcing transition to Limbo ending.");
#endif
                if (SceneLoader.Instance != null)
                {
                    SceneLoader.Instance.LoadSceneByName(endingLimboSceneName);
                }
                return;
            }
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.ReloadCurrentScene();
        }
    }
}
