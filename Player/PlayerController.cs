using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerController.cs
/// 所屬模組: Player
/// 責任描述: 管理 Meta Quest XR Rig 之移動、轉向、瞬移與雙手射線互動器之啟閉狀態。
/// 架構分層: Player / Controller
/// 聯動關係:
///   - 事件監聽: EventBus (PlayerStatusChangedEvent, HUDStatusChangedEvent)
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: XR Origin (XR Rig) 根物件。
/// ============================================================================
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("XR Ray Interactors")]
    [SerializeField] private GameObject leftNearFarInteractor;
    [SerializeField] private GameObject rightNearFarInteractor;

    [Header("Locomotion Modules")]
    [SerializeField] private GameObject moveModule;
    [SerializeField] private GameObject turnModule;
    [SerializeField] private GameObject teleportModule;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerStatusChangedEvent>(OnPlayerStatusChanged);
        EventBus.Subscribe<HUDStatusChangedEvent>(OnHUDStatusChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerStatusChangedEvent>(OnPlayerStatusChanged);
        EventBus.Unsubscribe<HUDStatusChangedEvent>(OnHUDStatusChanged);
    }

    private void OnPlayerStatusChanged(PlayerStatusChangedEvent e)
    {
        ToggleModule(moveModule, e.IsEnabled);
        ToggleModule(turnModule, e.IsEnabled);
        ToggleModule(teleportModule, e.IsEnabled);

#if UNITY_EDITOR
        Debug.Log($"[PlayerController] Locomotion modules toggled: {(e.IsEnabled ? "Enabled" : "Disabled")}");
#endif
    }

    private void OnHUDStatusChanged(HUDStatusChangedEvent e)
    {
        ToggleModule(leftNearFarInteractor, e.IsEnabled);
        ToggleModule(rightNearFarInteractor, e.IsEnabled);
    }

    private void ToggleModule(GameObject target, bool isEnabled)
    {
        if (target != null)
        {
            target.SetActive(isEnabled);
        }
    }
}
