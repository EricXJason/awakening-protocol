using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// ============================================================================
/// 檔案名稱: PowerQuizManager.cs
/// 所屬模組: Managers/SubManagers
/// 責任描述: 具體運思期 (Concrete Operational Stage) 能量算式謎題判定與橋樑升降過場。
/// 架構分層: Managers / Local SubManager
/// 聯動關係:
///   - 依賴: LevelManager_ConcreteOperationalStage, AudioManager, DOTween
/// ============================================================================
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class PowerQuizManager : MonoBehaviour
{
    public enum QuizType
    {
        Quiz_A, // 加減算式
        Quiz_B, // 先乘除後加減
        Quiz_C  // 括號運算
    }

    [Header("Quiz Configuration")]
    [SerializeField] private QuizType currentQuizType;
    [SerializeField] private string sfxKey_Check = "Check";
    [SerializeField] private string sfxKey_Correct = "CorrectAnswer";
    [SerializeField] private string sfxKey_Incorrect = "IncorrectAnswer";

    [Header("Visual Components")]
    [SerializeField] private TextMeshPro equationText;
    [SerializeField] private GameObject[] batteries;

    [Header("Bridge Animation Settings")]
    [SerializeField] private GameObject bridge;
    [SerializeField] private float yOffset = 1.0f;
    [SerializeField] private float animationTime = 1.5f;

    private BoxCollider targetCollider;
    private readonly List<BatteryController> insertedBatteries = new List<BatteryController>();
    private readonly Dictionary<GameObject, (Vector3 pos, Quaternion rot)> initialTransforms = new Dictionary<GameObject, (Vector3, Quaternion)>();

    private float targetValue = 0f;
    private float currentTotal = 0f;
    private int maxRequiredCount = 0;
    private bool isProcessing = false;
    private Tween bridgeTween;

    public event System.Action OnQuizSolved;

    private void Awake()
    {
        targetCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        InitializeQuizSettings();
    }

    private void OnDestroy()
    {
        bridgeTween?.Kill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isProcessing) return;

        if (other.TryGetComponent<BatteryController>(out var battery))
        {
            if (insertedBatteries.Contains(battery)) return;

            if (other.TryGetComponent<XRGrabInteractable>(out var grab))
            {
                if (grab.isSelected) return;
            }

            insertedBatteries.Add(battery);
            other.gameObject.SetActive(false);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(sfxKey_Check);
            }

            CalculateTotal();
            RenderPanel();
            CheckQuizStatus();
        }
    }

    private void InitializeQuizSettings()
    {
        switch (currentQuizType)
        {
            case QuizType.Quiz_A: targetValue = 5.0f; maxRequiredCount = 3; break;
            case QuizType.Quiz_B: targetValue = 4.0f; maxRequiredCount = 2; break;
            case QuizType.Quiz_C: targetValue = 8.0f; maxRequiredCount = 3; break;
        }

        if (batteries != null)
        {
            foreach (GameObject b in batteries)
            {
                if (b != null)
                {
                    initialTransforms[b] = (b.transform.localPosition, b.transform.localRotation);
                }
            }
        }

        if (bridge != null) bridge.SetActive(false);
        if (targetCollider != null) targetCollider.isTrigger = true;

        RenderPanel();
    }

    private void CalculateTotal()
    {
        currentTotal = 0f;

        if (currentQuizType == QuizType.Quiz_A)
        {
            float v1 = insertedBatteries.Count > 0 ? insertedBatteries[0].GetEnergyValue() : 0f;
            float v2 = insertedBatteries.Count > 1 ? insertedBatteries[1].GetEnergyValue() : 0f;
            float v3 = insertedBatteries.Count > 2 ? insertedBatteries[2].GetEnergyValue() : 0f;
            currentTotal = v1 + v2 - v3;
        }
        else if (currentQuizType == QuizType.Quiz_B)
        {
            string op1 = insertedBatteries.Count > 0 ? insertedBatteries[0].GetSymbolValue() : string.Empty;
            string op2 = insertedBatteries.Count > 1 ? insertedBatteries[1].GetSymbolValue() : string.Empty;

            float term1 = 3f;
            float term2 = 2f;
            float term3 = 1f;

            if (op1 == "×") { term1 *= term2; term2 = 0f; }
            if (op1 == "÷" && term2 != 0f) { term1 /= term2; term2 = 0f; }

            if (op2 == "×") { term2 *= term3; term3 = 0f; }
            if (op2 == "÷" && term3 != 0f) { term2 /= term3; term3 = 0f; }

            if (op1 == "×" || op1 == "÷")
            {
                if (op2 == "+") currentTotal = term1 + term3;
                else if (op2 == "-") currentTotal = term1 - term3;
                else currentTotal = term1;
            }
            else
            {
                float leftSide = op1 == "+" ? (3f + 2f) : (3f - 2f);
                if (op2 == "+") currentTotal = leftSide + term3;
                else if (op2 == "-") currentTotal = leftSide - term3;
                else if (op2 == "×") currentTotal = op1 == "+" ? 3f + (2f * 1f) : 3f - (2f * 1f);
                else if (op2 == "÷") currentTotal = op1 == "+" ? 3f + (2f / 1f) : 3f - (2f / 1f);
                else currentTotal = leftSide;
            }
        }
        else if (currentQuizType == QuizType.Quiz_C)
        {
            string op1 = insertedBatteries.Count > 0 ? insertedBatteries[0].GetSymbolValue() : string.Empty;
            string op2 = insertedBatteries.Count > 1 ? insertedBatteries[1].GetSymbolValue() : string.Empty;
            float num = insertedBatteries.Count > 2 ? insertedBatteries[2].GetEnergyValue() : 0f;

            if (insertedBatteries.Count == 3)
            {
                float bracketResult = op1 == "+" ? (5f + 1f) : (5f - 1f);
                if (op1 == "×") bracketResult = 5f * 1f;
                if (op1 == "÷") bracketResult = 5f / 1f;

                if (op2 == "+") currentTotal = bracketResult + num;
                else if (op2 == "-") currentTotal = bracketResult - num;
                else if (op2 == "×") currentTotal = bracketResult * num;
                else if (op2 == "÷" && num != 0f) currentTotal = bracketResult / num;
            }
        }
    }

    private void RenderPanel()
    {
        if (equationText == null) return;

        if (currentQuizType == QuizType.Quiz_A)
        {
            string s1 = insertedBatteries.Count > 0 ? insertedBatteries[0].GetSymbolValue() : "_";
            string s2 = insertedBatteries.Count > 1 ? insertedBatteries[1].GetSymbolValue() : "_";
            string s3 = insertedBatteries.Count > 2 ? insertedBatteries[2].GetSymbolValue() : "_";
            equationText.text = $"TARGET: {targetValue}\n[{s1}] + [{s2}] - [{s3}] = {currentTotal}";
        }
        else if (currentQuizType == QuizType.Quiz_B)
        {
            string op1 = insertedBatteries.Count > 0 ? insertedBatteries[0].GetSymbolValue() : "_";
            string op2 = insertedBatteries.Count > 1 ? insertedBatteries[1].GetSymbolValue() : "_";
            equationText.text = $"TARGET: {targetValue}\n3 [{op1}] 2 [{op2}] 1 = {currentTotal}";
        }
        else if (currentQuizType == QuizType.Quiz_C)
        {
            string op1 = insertedBatteries.Count > 0 ? insertedBatteries[0].GetSymbolValue() : "_";
            string op2 = insertedBatteries.Count > 1 ? insertedBatteries[1].GetSymbolValue() : "_";
            string s3 = insertedBatteries.Count > 2 ? insertedBatteries[2].GetSymbolValue() : "_";
            equationText.text = $"TARGET: {targetValue}\n( 5 [{op1}] 1 ) [{op2}] [{s3}] = {currentTotal}";
        }
    }

    private void CheckQuizStatus()
    {
        if (insertedBatteries.Count >= maxRequiredCount)
        {
            StartCoroutine(EndQuizProcessRoutine(Mathf.Abs(currentTotal - targetValue) < 0.001f));
        }
    }

    private IEnumerator EndQuizProcessRoutine(bool isSuccess)
    {
        isProcessing = true;

        if (equationText != null)
        {
            equationText.color = isSuccess ? Color.cyan : Color.red;
        }

        if (isSuccess)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(sfxKey_Correct);
            }
            if (targetCollider != null)
            {
                targetCollider.enabled = false;
            }

            if (bridge != null)
            {
                bridge.SetActive(true);
                bridgeTween?.Kill();
                bridgeTween = bridge.transform.DOMoveY(bridge.transform.position.y + yOffset, animationTime)
                    .SetEase(Ease.OutQuad);
            }

            OnQuizSolved?.Invoke();
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(sfxKey_Incorrect);
            }
        }

        yield return new WaitForSeconds(1.0f);

        if (!isSuccess)
        {
            if (equationText != null) equationText.color = Color.white;
            insertedBatteries.Clear();
            currentTotal = 0f;
            RenderPanel();
        }

        if (batteries != null)
        {
            foreach (GameObject b in batteries)
            {
                if (b != null)
                {
                    b.transform.DOKill();
                    b.transform.localPosition = initialTransforms[b].pos;
                    b.transform.localRotation = initialTransforms[b].rot;
                    b.SetActive(true);
                }
            }
        }

        isProcessing = false;
    }
}
