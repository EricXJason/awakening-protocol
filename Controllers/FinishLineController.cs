using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: FinishLineController.cs
/// 所屬模組: Controllers
/// 責任描述: 關卡終點線觸發器，抵達時通知關卡管理器達成通關條件。
/// 架構分層: Controllers / Trigger
/// 聯動關係:
///   - 依賴: BaseLevelManager (DIP 解耦)
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Collider))]
public class FinishLineController : MonoBehaviour
{
    [Header("Level Manager Reference")]
    [SerializeField] private BaseLevelManager targetLevelManager;

    private Collider finishCollider;
    private bool hasTriggered = false;

    public event System.Action OnPlayerCrossed;

    private void Awake()
    {
        finishCollider = GetComponent<Collider>();
        if (targetLevelManager == null)
        {
            targetLevelManager = FindObjectOfType<BaseLevelManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;
        if (finishCollider != null) finishCollider.enabled = false;

        OnPlayerCrossed?.Invoke();

        if (targetLevelManager != null)
        {
            targetLevelManager.CompleteLevel();
        }
    }
}
