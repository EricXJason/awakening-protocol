using System.Collections;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: HUDController.cs
/// 所屬模組: UI
/// 責任描述: VR 抬頭顯示器 (HUD) 淡入淡出視圖，響應 HUDStatusChangedEvent。
/// 架構分層: UI / Views
/// ============================================================================
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class HUDController : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string sfxKey = "HUD";

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<HUDStatusChangedEvent>(OnHUDStatusChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<HUDStatusChangedEvent>(OnHUDStatusChanged);
    }

    private void OnHUDStatusChanged(HUDStatusChangedEvent e)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(e.IsEnabled ? 1f : 0f));

        if (e.IsEnabled && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxKey);
        }
    }

    private IEnumerator FadeRoutine(float target)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }
}
