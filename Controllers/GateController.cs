using DG.Tweening;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: GateController.cs
/// 所屬模組: Controllers
/// 責任描述: 通道閘門雙向滑動開門動畫與觸發換關。實作 IBarrierController 契約。
/// 架構分層: Controllers / Barrier
/// 聯動關係:
///   - 介面實作: IBarrierController
///   - 依賴: SceneLoader, DOTween
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Collider))]
public class GateController : MonoBehaviour, IBarrierController
{
    [Header("Door Components")]
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    [Header("Animation Settings")]
    [SerializeField] private float xOffset = 2f;
    [SerializeField] private float animationTime = 1.5f;

    private Collider gateCollider;
    private Tween rightDoorTween;
    private Tween leftDoorTween;
    private bool isLoadingScene = false;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        gateCollider = GetComponent<Collider>();
    }

    private void OnDestroy()
    {
        rightDoorTween?.Kill();
        leftDoorTween?.Kill();
    }

    public void Open()
    {
        OpenGate();
    }

    public void Close()
    {
        IsOpen = false;
        rightDoorTween?.Kill();
        leftDoorTween?.Kill();
        if (gateCollider != null) gateCollider.enabled = false;
    }

    public void OpenGate()
    {
        IsOpen = true;
        if (gateCollider != null) gateCollider.enabled = true;

        rightDoorTween?.Kill();
        leftDoorTween?.Kill();

        if (rightDoor != null)
        {
            float targetRightX = rightDoor.transform.position.x + xOffset;
            rightDoorTween = rightDoor.transform.DOMoveX(targetRightX, animationTime).SetEase(Ease.InSine);
        }

        if (leftDoor != null)
        {
            float targetLeftX = leftDoor.transform.position.x - xOffset;
            leftDoorTween = leftDoor.transform.DOMoveX(targetLeftX, animationTime).SetEase(Ease.InSine);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLoadingScene || !other.CompareTag("Player")) return;

        isLoadingScene = true;
        if (gateCollider != null) gateCollider.enabled = false;

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadNextScene();
        }
    }
}
