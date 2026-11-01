using System;

/// <summary>
/// ============================================================================
/// 檔案名稱: ILevelManager.cs
/// 所屬模組: Managers/Interfaces
/// 責任描述: 規範各階段認知發展關卡管理器之核心契約 (DIP)。
///           統一定義各關卡之初始化、通關判定與狀態生命週期。
/// 架構分層: Managers / Interfaces
/// ============================================================================
/// </summary>
public interface ILevelManager
{
    int StageIndex { get; }
    string StageName { get; }
    bool IsCompleted { get; }

    void InitializeLevel();
    void CompleteLevel();
}
