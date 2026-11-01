using DG.Tweening;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: BlockCheckerController.cs
/// 所屬模組: Controllers
/// 責任描述: 前運思期 (Preoperational Stage) 積木形狀配對檢驗與對應橋樑顯現。
/// 架構分層: Controllers / Puzzle
/// 聯動關係:
///   - 依賴: AudioManager, DOTween
///   - 協同: LevelManager_PreoperationalStage
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Collider))]
public class BlockCheckerController : MonoBehaviour
{
    [Header("Block Comparison Targets")]
    [SerializeField] private Transform[] blocks = new Transform[4];

    [Header("Target Bridge Parts")]
    [SerializeField] private GameObject[] bridgeParts = new GameObject[4];

    [Header("Movement Animation Settings")]
    [SerializeField] private float yOffset = -5f;
    [SerializeField] private string sfxKey = "Check";
    [SerializeField] private float animationTime = 1.5f;

    private Tween showDoorTween;
    private bool isPlaying = false;

    public event System.Action OnPuzzleCompleted;

    private void OnDestroy()
    {
        showDoorTween?.Kill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaying) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxKey);
        }

        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] == null) continue;

            string targetName = $"Block_0{i + 1}";
            if (other.name == targetName || other.transform.name == targetName)
            {
                isPlaying = true;

                for (int j = 0; j < bridgeParts.Length; j++)
                {
                    if (bridgeParts[j] != null) bridgeParts[j].SetActive(false);
                }

                showDoorTween?.Kill();

                GameObject currentPart = bridgeParts[i];
                if (currentPart != null)
                {
                    currentPart.SetActive(true);

                    float finalY = currentPart.transform.position.y;
                    float startY = finalY + yOffset;

                    currentPart.transform.position = new Vector3(
                        currentPart.transform.position.x,
                        startY,
                        currentPart.transform.position.z
                    );

                    showDoorTween = currentPart.transform
                        .DOMoveY(finalY, animationTime)
                        .SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            isPlaying = false;
                            OnPuzzleCompleted?.Invoke();
                        });
                }
                break;
            }
        }
    }
}
