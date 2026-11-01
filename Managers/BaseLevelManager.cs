using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: BaseLevelManager.cs
/// 所屬模組: Managers
/// 責任描述: 認知發展關卡管理器之抽象基底類別 (Template Method Pattern)。
///           萃取 6 個關卡共通之生命週期、過場動畫時序綁定與通關狀態維護。
///           消除重複代碼 (DRY)，各具體關卡僅需專注於該階段之特色解謎與規則判定。
/// 架構分層: Managers / Base
/// 聯動關係:
///   - 介面實作: ILevelManager
///   - 依賴: StageSequenceController, GameManager
/// ============================================================================
/// </summary>
public abstract class BaseLevelManager : MonoBehaviour, ILevelManager
{
    [Header("Stage Configuration")]
    [SerializeField] protected int stageIndex;
    [SerializeField] protected string stageName = "Stage";

    [Header("Sequence and Animation Reference")]
    [SerializeField] protected StageSequenceController stageSequence;

    public int StageIndex => stageIndex;
    public string StageName => stageName;
    public bool IsCompleted { get; protected set; }

    protected virtual void Awake()
    {
        if (stageSequence == null)
        {
            stageSequence = GetComponent<StageSequenceController>();
            if (stageSequence == null)
            {
                stageSequence = FindObjectOfType<StageSequenceController>();
            }
        }
    }

    protected virtual void Start()
    {
        InitializeLevel();
    }

    public virtual void InitializeLevel()
    {
        IsCompleted = false;

        if (stageSequence != null)
        {
            stageSequence.OnOpeningCompleted += OnStagePlayable;
            stageSequence.OnEndingCompleted += OnStageTransitionNext;
        }
    }

    protected virtual void OnDestroy()
    {
        if (stageSequence != null)
        {
            stageSequence.OnOpeningCompleted -= OnStagePlayable;
            stageSequence.OnEndingCompleted -= OnStageTransitionNext;
        }
    }

    public virtual void CompleteLevel()
    {
        if (IsCompleted) return;
        IsCompleted = true;

#if UNITY_EDITOR
        Debug.Log($"[{GetType().Name}] Stage {stageIndex} ({stageName}) completed! Triggering sequence transition.");
#endif

        if (stageSequence != null)
        {
            stageSequence.TriggerEndingProcess();
        }
    }

    /// <summary>
    /// 當開場動畫與語音結束、正式解鎖玩家操作時觸發之生命週期回呼
    /// </summary>
    protected virtual void OnStagePlayable()
    {
    }

    /// <summary>
    /// 當結尾過場動畫播畢、即將切換下一關卡時觸發之生命週期回呼
    /// </summary>
    protected virtual void OnStageTransitionNext()
    {
    }
}
