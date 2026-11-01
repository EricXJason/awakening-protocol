using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: LevelManager_LimboStage.cs
/// 所屬模組: Managers
/// 責任描述: Stage 0: 邊界迷失序章 (Limbo Stage) 關卡管理器。
///           管理引導光球巡航路徑、環境暗角淡入淡出與第一階段過渡。
///           作為場景中介者 (Mediator)，監聽 GuideLightBallController 事件，不使用單例。
/// 架構分層: Managers / Implementation
/// 聯動關係:
///   - 繼承: BaseLevelManager
///   - 依賴: GuideLightBallController, StageSequenceController
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景中的「[LevelManager]」。
/// ============================================================================
/// </summary>
public class LevelManager_LimboStage : BaseLevelManager
{
    [Header("Limbo Stage References")]
    [SerializeField] private GuideLightBallController guideLightBall;

    protected override void Awake()
    {
        base.Awake();
        stageIndex = 0;
        stageName = "LimboStage";
    }

    protected override void Start()
    {
        base.Start();

        if (guideLightBall != null)
        {
            guideLightBall.OnNavigationCompleted += OnLightBallArrived;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (guideLightBall != null)
        {
            guideLightBall.OnNavigationCompleted -= OnLightBallArrived;
        }
    }

    private void OnLightBallArrived()
    {
        CompleteLevel();
    }
}
