using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: ElectricGridController.cs
/// 所屬模組: Controllers
/// 責任描述: 高壓電網障礙觸發器，誤觸自動重新加載關卡。
/// 架構分層: Controllers / Hazard Trigger
/// 聯動關係:
///   - 依賴: SceneLoader
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Collider))]
public class ElectricGridController : MonoBehaviour
{
    private Collider gridCollider;
    private bool hasTriggered = false;

    private void Awake()
    {
        gridCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;
        if (gridCollider != null) gridCollider.enabled = false;

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.ReloadCurrentScene();
        }
    }
}
