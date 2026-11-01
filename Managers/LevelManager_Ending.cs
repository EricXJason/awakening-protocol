using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: LevelManager_Ending.cs
/// 所屬模組: Managers
/// 責任描述: Stage 5: 結局展示與結算 (Ending Stage) 關卡管理器。
///           負責都市甦醒或迷失重啟之終局過場時序、製作人員名單滾動與返回主選單。
///           場景專屬生命週期控制器，不使用單例。
/// 架構分層: Managers / Implementation
/// 聯動關係:
///   - 繼承: BaseLevelManager
///   - 依賴: StageSequenceController, SceneLoader
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景中的「[LevelManager]」。
/// ============================================================================
/// </summary>
public class LevelManager_Ending : BaseLevelManager
{
    [Header("Ending Sequence Settings")]
    [SerializeField] private string mainMenuSceneName = "00_MainMenu";
    [SerializeField] private float returnToMenuDelay = 8f;

    protected override void Awake()
    {
        base.Awake();
        stageIndex = 5;
        stageName = "EndingStage";
    }

    protected override void OnStageTransitionNext()
    {
        base.OnStageTransitionNext();
        Invoke(nameof(ReturnToMainMenu), returnToMenuDelay);
    }

    private void ReturnToMainMenu()
    {
#if UNITY_EDITOR
        Debug.Log("[LevelManager_Ending] Returning to Main Menu.");
#endif
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneByName(mainMenuSceneName);
        }
    }
}
