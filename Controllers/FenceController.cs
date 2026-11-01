using DG.Tweening;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: FenceController.cs
/// 所屬模組: Controllers
/// 責任描述: 阻擋通行之升降防護柵欄控制器。實作 IBarrierController 契約。
/// 架構分層: Controllers / Barrier
/// 聯動關係:
///   - 介面實作: IBarrierController
///   - 依賴: DOTween
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Collider))]
public class FenceController : MonoBehaviour, IBarrierController
{
    [Header("Position Configuration")]
    [SerializeField] private Vector3 hidePosition;

    [Header("Animation Settings")]
    [SerializeField] private float animationTime = 1.5f;

    private Vector3 showPosition;
    private Collider fenceCollider;
    private Tween moveTween;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        fenceCollider = GetComponent<Collider>();
        showPosition = transform.position;
        IsOpen = false;
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
    }

    public void Open()
    {
        HideFence();
    }

    public void Close()
    {
        ShowFence();
    }

    public void ShowFence()
    {
        IsOpen = false;
        moveTween?.Kill();
        if (fenceCollider != null) fenceCollider.enabled = true;
        moveTween = transform.DOMove(showPosition, animationTime).SetEase(Ease.InSine);
    }

    public void HideFence()
    {
        IsOpen = true;
        moveTween?.Kill();
        moveTween = transform.DOMove(hidePosition, animationTime)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                if (fenceCollider != null) fenceCollider.enabled = false;
            });
    }
}
