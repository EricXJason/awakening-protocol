using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: LevelManager_SensorimotorStage.cs
/// 所屬模組: Managers
/// 責任描述: Stage 1: 感覺動作期 (Sensorimotor Stage) 音階燈球謎題邏輯判定與通關管理。
///           玩家需依照特定音高順序觸碰懸浮燈球，全部正確後觸發通關過場。
///           作為場景中介者 (Mediator)，監聽 LightBallController 事件，不使用單例。
/// 架構分層: Managers / Implementation
/// 聯動關係:
///   - 繼承: BaseLevelManager
///   - 下游監聽: LightBallController
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景中的「[LevelManager]」。
/// ============================================================================
/// </summary>
public class LevelManager_SensorimotorStage : BaseLevelManager
{
    [Header("Pitch Puzzle Configuration")]
    [SerializeField] private int targetTotalBalls = 8;
    [SerializeField] private LightBallController[] lightBalls;

    private int currentExpectedBallId = 1;

    protected override void Awake()
    {
        base.Awake();
        stageIndex = 1;
        stageName = "SensorimotorStage";
    }

    protected override void Start()
    {
        base.Start();

        if (lightBalls == null || lightBalls.Length == 0)
        {
            lightBalls = FindObjectsOfType<LightBallController>();
        }

        if (lightBalls != null)
        {
            foreach (var ball in lightBalls)
            {
                if (ball != null)
                {
                    ball.OnBallTouched += HandleBallTouched;
                }
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (lightBalls != null)
        {
            foreach (var ball in lightBalls)
            {
                if (ball != null)
                {
                    ball.OnBallTouched -= HandleBallTouched;
                }
            }
        }
    }

    /// <summary>
    /// 仲裁玩家觸碰音階燈球順序是否正確
    /// </summary>
    private void HandleBallTouched(LightBallController ball, int ballId)
    {
        if (ballId == currentExpectedBallId)
        {
            currentExpectedBallId++;
#if UNITY_EDITOR
            Debug.Log($"[SensorimotorStage] Correct pitch touched: {ballId}, next target: {currentExpectedBallId}");
#endif
            if (currentExpectedBallId > targetTotalBalls)
            {
                CompleteLevel();
            }
        }
        else if (ballId < currentExpectedBallId)
        {
            // 已正確點擊過的音階重複點擊不重置
        }
        else
        {
            currentExpectedBallId = 1;
#if UNITY_EDITOR
            Debug.LogWarning("[SensorimotorStage] Incorrect touch sequence, resetting all pitches!");
#endif
            ResetAllBalls();
        }
    }

    private void ResetAllBalls()
    {
        if (lightBalls != null)
        {
            foreach (var b in lightBalls)
            {
                if (b != null) b.ResetLightBall();
            }
        }
    }
}
