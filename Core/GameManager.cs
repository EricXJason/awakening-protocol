using System;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: GameManager.cs
/// 所屬模組: Core
/// 責任描述: 全域常駐核心遊戲狀態管理器。實作 IGameManager 契約，管理語言設定、
///           失敗計數、操作權限，並透過 EventBus 廣播領域事件。
/// 架構分層: Core / Manager
/// 聯動關係:
///   - 介面實作: IGameManager
///   - 事件廣播: EventBus (PlayerStatusChangedEvent, HUDStatusChangedEvent, SubtitleTriggeredEvent, ScreenDimmerTriggeredEvent)
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景根節點下的空物件，命名為「[GameManager]」。
/// 2. 生命週期: 腳本內建 DontDestroyOnLoad，保持全域常駐。
/// ============================================================================
/// </summary>
public class GameManager : MonoBehaviour, IGameManager
{
    private static GameManager instance;

    public static GameManager Instance => instance;

    public int FailCount { get; private set; }
    public GameLanguage CurrentLanguage { get; private set; } = GameLanguage.Chinese;
    public bool IsPlayable { get; private set; } = true;

    // 原生 C# Action 事件 (保留相容性)
    public event Action<bool> OnPlayerStatusChanged;
    public event Action<bool> OnHUDStatusChanged;
    public event Action<string, float> OnSubtitleTriggered;
    public event Action<float, float> OnDimmerTriggered;
    public event Action OnPauseMenuRequested;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RequestPauseMenu()
    {
        OnPauseMenuRequested?.Invoke();
    }

    public void SetPlayableStatus(bool playable)
    {
        IsPlayable = playable;
    }

    public void SetCurrentLanguage(GameLanguage language)
    {
        CurrentLanguage = language;
    }

    public void AddFailCount()
    {
        FailCount++;
#if UNITY_EDITOR
        Debug.Log($"[GameManager] Fail count incremented, current count: {FailCount}");
#endif
    }

    public void ResetFailCount()
    {
        FailCount = 0;
    }

    public void SetPlayerStatus(bool status)
    {
        OnPlayerStatusChanged?.Invoke(status);
        EventBus.Publish(new PlayerStatusChangedEvent(status));
    }

    public void SetHUDStatus(bool status)
    {
        OnHUDStatusChanged?.Invoke(status);
        EventBus.Publish(new HUDStatusChangedEvent(status));
    }

    public void TriggerGlobalSubtitle(string text, float fadeOutDuration)
    {
        OnSubtitleTriggered?.Invoke(text, fadeOutDuration);
        EventBus.Publish(new SubtitleTriggeredEvent(text, fadeOutDuration));
    }

    public void TriggerGlobalDimmer(float targetAlpha, float duration)
    {
        OnDimmerTriggered?.Invoke(targetAlpha, duration);
        EventBus.Publish(new ScreenDimmerTriggeredEvent(targetAlpha, duration));
    }
}
