using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ============================================================================
/// 檔案名稱: ScreenDimmerController.cs
/// 所屬模組: UI
/// 責任描述: 全景視野暗角遮罩控制器，響應 ScreenDimmerTriggeredEvent。
/// 架構分層: UI / Views
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Image))]
public class ScreenDimmerController : MonoBehaviour
{
    private Image dimmerImage;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        dimmerImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ScreenDimmerTriggeredEvent>(OnDimmerTriggered);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScreenDimmerTriggeredEvent>(OnDimmerTriggered);
    }

    private void OnDimmerTriggered(ScreenDimmerTriggeredEvent e)
    {
        if (dimmerImage == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (e.FadeDuration <= 0f)
        {
            Color c = dimmerImage.color;
            c.a = e.TargetAlpha;
            dimmerImage.color = c;
        }
        else
        {
            fadeCoroutine = StartCoroutine(FadeRoutine(e.TargetAlpha, e.FadeDuration));
        }
    }

    private IEnumerator FadeRoutine(float destinationAlpha, float duration)
    {
        float elapsed = 0f;
        float startAlpha = dimmerImage.color.a;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color c = dimmerImage.color;
            c.a = Mathf.Lerp(startAlpha, destinationAlpha, elapsed / duration);
            dimmerImage.color = c;
            yield return null;
        }

        Color final = dimmerImage.color;
        final.a = destinationAlpha;
        dimmerImage.color = final;
    }
}
