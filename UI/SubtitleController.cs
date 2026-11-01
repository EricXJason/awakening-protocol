using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: SubtitleController.cs
/// 所屬模組: UI
/// 責任描述: 全域 3D/WorldSpace 字幕視圖，響應 SubtitleTriggeredEvent。
/// 架構分層: UI / Views
/// ============================================================================
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class SubtitleController : MonoBehaviour
{
    private TextMeshPro subtitleText;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        subtitleText = GetComponent<TextMeshPro>();
        if (subtitleText != null) subtitleText.text = string.Empty;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<SubtitleTriggeredEvent>(OnSubtitleTriggered);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<SubtitleTriggeredEvent>(OnSubtitleTriggered);
    }

    private void OnSubtitleTriggered(SubtitleTriggeredEvent e)
    {
        if (subtitleText == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (!string.IsNullOrEmpty(e.SubtitleText))
        {
            subtitleText.text = e.SubtitleText;
            Color c = subtitleText.color;
            c.a = 1f;
            subtitleText.color = c;
        }
        else
        {
            if (e.FadeOutDuration > 0f && gameObject.activeInHierarchy)
            {
                fadeCoroutine = StartCoroutine(FadeOutRoutine(e.FadeOutDuration));
            }
            else
            {
                subtitleText.text = string.Empty;
            }
        }
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float elapsed = 0f;
        Color startColor = subtitleText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color c = subtitleText.color;
            c.a = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
            subtitleText.color = c;
            yield return null;
        }

        subtitleText.text = string.Empty;
        Color final = subtitleText.color;
        final.a = 1f;
        subtitleText.color = final;
    }
}
