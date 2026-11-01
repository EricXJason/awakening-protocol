using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// ============================================================================
/// 檔案名稱: ReturnToAnchor.cs
/// 所屬模組: Infrastructure/Interaction
/// 責任描述: XR 抓取物件放手後平滑回彈歸位至原始錨點 (DOTween Ease.OutBack)。
/// 架構分層: Infrastructure / XR Interaction Utility
/// 聯動關係:
///   - 依賴: XRGrabInteractable, DOTween
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class ReturnToAnchor : MonoBehaviour
{
    [Header("Return Animation Settings")]
    [SerializeField] private float returnDuration = 0.5f;
    [SerializeField] private Ease returnEase = Ease.OutBack;

    private Vector3 originPosition;
    private Quaternion originRotation;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Tween moveTween;
    private Tween rotateTween;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        originPosition = transform.localPosition;
        originRotation = transform.localRotation;

        if (rb != null) rb.isKinematic = true;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
        rotateTween?.Kill();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        moveTween?.Kill();
        rotateTween?.Kill();
        if (rb != null) rb.isKinematic = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (rb != null) rb.isKinematic = true;

        moveTween?.Kill();
        rotateTween?.Kill();

        moveTween = transform.DOLocalMove(originPosition, returnDuration).SetEase(returnEase);
        rotateTween = transform.DOLocalRotateQuaternion(originRotation, returnDuration).SetEase(returnEase);
    }
}
