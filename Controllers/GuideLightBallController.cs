using DG.Tweening;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: GuideLightBallController.cs
/// 所屬模組: Controllers
/// 責任描述: Limbo 階段引導光球航點巡航與終點通關通知。
/// 架構分層: Controllers / Navigation
/// 聯動關係:
///   - 依賴: LevelManager_LimboStage, DOTween
/// ============================================================================
/// </summary>
public class GuideLightBallController : MonoBehaviour
{
    [Header("Navigation Configuration")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject gate;
    [SerializeField] private float moveSpeed = 3f;

    private Tween moveTween;
    private int currentWaypointIndex = 0;
    private bool isFinished = false;

    public event System.Action OnNavigationCompleted;

    private void Start()
    {
        if (gate != null) gate.SetActive(false);
        MoveToNextWaypoint();
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex >= waypoints.Length)
        {
            HandleFinishing();
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        float duration = distance / Mathf.Max(moveSpeed, 0.1f);

        moveTween?.Kill();
        moveTween = transform.DOMove(target.position, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                currentWaypointIndex++;
                MoveToNextWaypoint();
            });
    }

    private void HandleFinishing()
    {
        if (isFinished) return;

        isFinished = true;
        moveTween?.Kill();

        if (gate != null) gate.SetActive(true);

        OnNavigationCompleted?.Invoke();
    }
}
