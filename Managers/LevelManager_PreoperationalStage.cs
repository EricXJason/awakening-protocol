using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: LevelManager_PreoperationalStage.cs
/// 所屬模組: Managers
/// 責任描述: Stage 2: 前運思期 (Preoperational Stage) 幾何積木配對驗證與通關管理。
///           玩家需將幾何實體正確放置於對應插槽，全部吻合後升起浮空橋樑。
///           作為場景中介者 (Mediator)，監聽 BlockCheckerController 事件，不使用單例。
/// 架構分層: Managers / Implementation
/// 聯動關係:
///   - 繼承: BaseLevelManager
///   - 依賴: BlockCheckerController
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景中的「[LevelManager]」。
/// ============================================================================
/// </summary>
public class LevelManager_PreoperationalStage : BaseLevelManager
{
    [Header("Block Puzzle Reference")]
    [SerializeField] private BlockCheckerController blockChecker;

    protected override void Awake()
    {
        base.Awake();
        stageIndex = 2;
        stageName = "PreoperationalStage";
    }

    protected override void Start()
    {
        base.Start();

        if (blockChecker == null)
        {
            blockChecker = FindObjectOfType<BlockCheckerController>();
        }

        if (blockChecker != null)
        {
            blockChecker.OnPuzzleCompleted += HandlePuzzleCompleted;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (blockChecker != null)
        {
            blockChecker.OnPuzzleCompleted -= HandlePuzzleCompleted;
        }
    }

    private void HandlePuzzleCompleted()
    {
        CompleteLevel();
    }
}
