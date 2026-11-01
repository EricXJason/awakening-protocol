using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: StageSequenceController.cs
/// 所屬模組: Managers
/// 責任描述: 統一管理皮亞傑認知發展各關卡的開場/結尾過場時序、語音播送、字幕同步、
///           視野遮罩與玩家操作鎖定。實作 IStageAnimationService 契約，
///           徹底消除原本 6 個關卡重複冗餘的 AnimationManager (遵循 DRY 原則)。
/// 架構分層: Managers / Sequence
/// 聯動關係:
///   - 介面實作: IStageAnimationService
///   - 依賴: GameManager, AudioManager, EventBus
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 各關卡場景中的「[StageSequence]」或「[LevelManager]」物件。
/// ============================================================================
/// </summary>
public class StageSequenceController : MonoBehaviour, IStageAnimationService
{
    [Header("Audio and Voice Configuration")]
    [SerializeField] private string openingVoiceKey;
    [SerializeField] private string endingVoiceKey;
    [SerializeField] private float waitToPlayOpeningVoice = 1f;

    [Header("Vision Mask and Vignette Configuration")]
    [SerializeField] [Range(0f, 1f)] private float dimTargetAlpha = 0.85f;
    [SerializeField] [Range(0f, 1f)] private float normalAmbientAlpha = 0f;
    [SerializeField] private float openingFadeInDuration = 0f;
    [SerializeField] private float openingFadeOutDuration = 2f;
    [SerializeField] private float endingFadeInDuration = 0.75f;
    [SerializeField] private float endingFadeOutDuration = 2f;

    [Header("Bilingual Subtitles - Opening")]
    [SerializeField] private string openingSubtitleCH;
    [SerializeField] private string openingSubtitleEN;

    [Header("Bilingual Subtitles - Ending")]
    [SerializeField] private string endingSubtitleCH;
    [SerializeField] private string endingSubtitleEN;

    [Header("Branch Voice Configuration (Based on Fail Count)")]
    [SerializeField] private bool useFailCountVoiceIndex = false;
    [SerializeField] private string[] openingSubtitlesCHArray;
    [SerializeField] private string[] openingSubtitlesENArray;

    public event Action OnOpeningStarted;
    public event Action OnOpeningCompleted;
    public event Action OnEndingStarted;
    public event Action OnEndingCompleted;

    private void Start()
    {
        if (GameManager.Instance != null && useFailCountVoiceIndex)
        {
            openingVoiceKey = $"{openingVoiceKey}_{GameManager.Instance.FailCount}";
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerStatus(false);
            GameManager.Instance.SetPlayableStatus(false);
        }

        TriggerOpeningProcess();
    }

    public void TriggerOpeningProcess()
    {
        StartCoroutine(OpeningSequenceRoutine());
    }

    public void TriggerEndingProcess()
    {
        OnEndingStarted?.Invoke();
        EventBus.Publish(new StagePhaseChangedEvent(StagePhase.EndingStarted));

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerStatus(false);
            GameManager.Instance.SetHUDStatus(false);
            GameManager.Instance.SetPlayableStatus(false);
        }

        StartCoroutine(EndingSequenceRoutine());
    }

    private IEnumerator OpeningSequenceRoutine()
    {
        OnOpeningStarted?.Invoke();
        EventBus.Publish(new StagePhaseChangedEvent(StagePhase.OpeningStarted));

        if (waitToPlayOpeningVoice > 0f)
            yield return new WaitForSeconds(waitToPlayOpeningVoice);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGlobalDimmer(dimTargetAlpha, openingFadeInDuration);
        }

        float voiceDuration = 0f;
        if (AudioManager.Instance != null && !string.IsNullOrWhiteSpace(openingVoiceKey))
        {
            var clip = AudioManager.Instance.GetClip(openingVoiceKey);
            if (clip != null)
            {
                AudioManager.Instance.PlaySFX(openingVoiceKey);
                voiceDuration = clip.length;
            }
        }

        string subtitle = ResolveOpeningSubtitle();
        if (GameManager.Instance != null && !string.IsNullOrEmpty(subtitle))
        {
            GameManager.Instance.ShowSubtitle(subtitle);
        }

        yield return new WaitForSeconds(voiceDuration);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.HideSubtitle();
            GameManager.Instance.TriggerGlobalDimmer(normalAmbientAlpha, openingFadeOutDuration);
            GameManager.Instance.SetPlayerStatus(true);
            GameManager.Instance.SetPlayableStatus(true);
        }

        yield return new WaitForSeconds(openingFadeOutDuration);

        OnOpeningCompleted?.Invoke();
        EventBus.Publish(new StagePhaseChangedEvent(StagePhase.OpeningCompleted));
    }

    private IEnumerator EndingSequenceRoutine()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGlobalDimmer(dimTargetAlpha, endingFadeInDuration);
        }

        yield return new WaitForSeconds(endingFadeInDuration);

        float voiceDuration = 0f;
        if (AudioManager.Instance != null && !string.IsNullOrWhiteSpace(endingVoiceKey))
        {
            var clip = AudioManager.Instance.GetClip(endingVoiceKey);
            if (clip != null)
            {
                AudioManager.Instance.PlaySFX(endingVoiceKey);
                voiceDuration = clip.length;
            }
        }

        string subtitle = ResolveEndingSubtitle();
        if (GameManager.Instance != null && !string.IsNullOrEmpty(subtitle))
        {
            GameManager.Instance.ShowSubtitle(subtitle);
        }

        yield return new WaitForSeconds(voiceDuration);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.HideSubtitle();
            GameManager.Instance.TriggerGlobalDimmer(1f, endingFadeOutDuration);
        }

        yield return new WaitForSeconds(endingFadeOutDuration);

        OnEndingCompleted?.Invoke();
        EventBus.Publish(new StagePhaseChangedEvent(StagePhase.EndingCompleted));

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadNextScene();
        }
    }

    private string ResolveOpeningSubtitle()
    {
        bool isChinese = (GameManager.Instance == null || GameManager.Instance.CurrentLanguage == GameLanguage.Chinese);
        int failCount = (GameManager.Instance != null) ? GameManager.Instance.FailCount : 0;

        if (useFailCountVoiceIndex)
        {
            var targetArray = isChinese ? openingSubtitlesCHArray : openingSubtitlesENArray;
            if (targetArray != null && targetArray.Length > 0)
            {
                int index = Mathf.Clamp(failCount, 0, targetArray.Length - 1);
                return targetArray[index];
            }
        }

        return isChinese ? openingSubtitleCH : openingSubtitleEN;
    }

    private string ResolveEndingSubtitle()
    {
        bool isChinese = (GameManager.Instance == null || GameManager.Instance.CurrentLanguage == GameLanguage.Chinese);
        return isChinese ? endingSubtitleCH : endingSubtitleEN;
    }
}
