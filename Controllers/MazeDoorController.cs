using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: MazeDoorController.cs
/// 所屬模組: Controllers
/// 責任描述: 形式運思期 (Formal Operational Stage) 迷宮生路/死路判定門。
///           玩家穿過門扉時發送選擇事件，由關卡管理器負責分支仲裁。
/// 架構分層: Controllers / Trigger
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Collider))]
public class MazeDoorController : MonoBehaviour
{
    [Header("Door Configuration")]
    [SerializeField] private bool isCorrectDoor = false;

    private bool hasTriggered = false;

    public event System.Action<bool> OnDoorSelected;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;
        OnDoorSelected?.Invoke(isCorrectDoor);
    }
}
