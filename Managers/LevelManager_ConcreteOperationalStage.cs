using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: LevelManager_ConcreteOperationalStage.cs
/// 所屬模組: Managers
/// 責任描述: Stage 3: 具體運思期 (Concrete Operational Stage) 能量電池等式仲裁與通關管理。
///           玩家需計算插槽目標電量並插入正確數值電池，達成數學平衡後觸發過場。
///           作為場景中介者 (Mediator)，監聽 PowerQuizManager 事件，不使用單例。
/// 架構分層: Managers / Implementation
/// 聯動關係:
///   - 繼承: BaseLevelManager
///   - 依賴: PowerQuizManager
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景中的「[LevelManager]」。
/// ============================================================================
/// </summary>
public class LevelManager_ConcreteOperationalStage : BaseLevelManager
{
    [Header("Power Quiz Reference")]
    [SerializeField] private PowerQuizManager powerQuiz;

    protected override void Awake()
    {
        base.Awake();
        stageIndex = 3;
        stageName = "ConcreteOperationalStage";
    }

    protected override void Start()
    {
        base.Start();

        if (powerQuiz == null)
        {
            powerQuiz = FindObjectOfType<PowerQuizManager>();
        }

        if (powerQuiz != null)
        {
            powerQuiz.OnQuizSolved += HandleQuizSolved;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (powerQuiz != null)
        {
            powerQuiz.OnQuizSolved -= HandleQuizSolved;
        }
    }

    private void HandleQuizSolved()
    {
        CompleteLevel();
    }
}
