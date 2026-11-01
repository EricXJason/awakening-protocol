/// <summary>
/// ============================================================================
/// 檔案名稱: IGameManager.cs
/// 所屬模組: Core
/// 責任描述: 定義遊戲全域核心狀態管理契約。
/// 架構分層: Core / Interfaces
/// ============================================================================
/// </summary>

public enum GameLanguage
{
    Chinese,
    English
}

public interface IGameManager
{
    int FailCount { get; }
    GameLanguage CurrentLanguage { get; }
    bool IsPlayable { get; }

    void SetPlayableStatus(bool playable);
    void SetCurrentLanguage(GameLanguage language);
    void AddFailCount();
    void ResetFailCount();

    void RequestPauseMenu();
    void SetPlayerStatus(bool status);
    void SetHUDStatus(bool status);
    void TriggerGlobalSubtitle(string text, float fadeOutDuration);
    void TriggerGlobalDimmer(float targetAlpha, float duration);
}
