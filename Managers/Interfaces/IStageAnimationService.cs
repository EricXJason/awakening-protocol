using System;

/// <summary>
/// ============================================================================
/// 檔案名稱: IStageAnimationService.cs
/// 所屬模組: Managers/Interfaces
/// 責任描述: 規範各關卡開場/結尾過場時序、視野暗角補間與字幕語音廣播之抽象契約 (DIP)。
/// 架構分層: Managers / Interfaces
/// ============================================================================
/// </summary>
public interface IStageAnimationService
{
    event Action OnOpeningStarted;
    event Action OnOpeningCompleted;
    event Action OnEndingStarted;
    event Action OnEndingCompleted;

    void TriggerOpeningProcess();
    void TriggerEndingProcess();
}
