/// <summary>
/// ============================================================================
/// 檔案名稱: VREvents.cs
/// 所屬模組: Infrastructure/EventBus/Events
/// 責任描述: 定義覺醒協議 (Awakening Protocol) 之全域領域事件資料載體。
/// 架構分層: Infrastructure / Domain Events
/// 邊界處理: 採用 readonly struct 杜絕 GC Allocation 與執行時期數值篡改
/// ============================================================================
/// </summary>

/// <summary>
/// 玩家可操作狀態異動事件 (控制移動/轉向/瞬移模組開關)
/// </summary>
public readonly struct PlayerStatusChangedEvent
{
    public readonly bool IsEnabled;

    public PlayerStatusChangedEvent(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}

/// <summary>
/// HUD 抬頭顯示器與射線互動器開關事件
/// </summary>
public readonly struct HUDStatusChangedEvent
{
    public readonly bool IsEnabled;

    public HUDStatusChangedEvent(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}

/// <summary>
/// 全域字幕播送事件
/// </summary>
public readonly struct SubtitleTriggeredEvent
{
    public readonly string SubtitleText;
    public readonly float FadeOutDuration;

    public SubtitleTriggeredEvent(string subtitleText, float fadeOutDuration)
    {
        SubtitleText = subtitleText;
        FadeOutDuration = fadeOutDuration;
    }
}

/// <summary>
/// 全域視野暗角/淡入淡出事件
/// </summary>
public readonly struct ScreenDimmerTriggeredEvent
{
    public readonly float TargetAlpha;
    public readonly float FadeDuration;

    public ScreenDimmerTriggeredEvent(float targetAlpha, float fadeDuration)
    {
        TargetAlpha = targetAlpha;
        FadeDuration = fadeDuration;
    }
}

/// <summary>
/// 關卡進程/開場與結尾動畫階段異動事件
/// </summary>
public readonly struct StagePhaseChangedEvent
{
    public readonly StagePhase Phase;

    public StagePhaseChangedEvent(StagePhase phase)
    {
        Phase = phase;
    }
}

public enum StagePhase
{
    OpeningStarted,
    OpeningCompleted,
    GameplayActive,
    EndingStarted,
    EndingCompleted
}
