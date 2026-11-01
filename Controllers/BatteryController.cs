using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// ============================================================================
/// 檔案名稱: BatteryController.cs
/// 所屬模組: Controllers
/// 責任描述: 具體運思期 (Concrete Operational Stage) 能量等式謎題之電池物件。
///           封裝數值能量與運算子代號，提供 PowerQuizManager 插槽讀取判定。
/// 架構分層: Controllers / Interactable
/// ============================================================================
/// </summary>
public enum BatteryType
{
    Type_A, // 數值 1
    Type_B, // 數值 2
    Type_C, // 數值 3
    Type_D, // 數值 5
    Type_E, // 運算子 +
    Type_F, // 運算子 -
    Type_G, // 運算子 ×
    Type_H  // 運算子 ÷
}

[RequireComponent(typeof(XRGrabInteractable))]
public class BatteryController : MonoBehaviour
{
    [Header("Battery Type")]
    [SerializeField] private BatteryType currentBatteryType;

    private float energyValue = 0f;
    private string symbolValue = string.Empty;
    private bool isNumber = true;

    private void Awake()
    {
        InitializeBattery();
    }

    private void InitializeBattery()
    {
        switch (currentBatteryType)
        {
            case BatteryType.Type_A: energyValue = 1f; symbolValue = "1"; isNumber = true; break;
            case BatteryType.Type_B: energyValue = 2f; symbolValue = "2"; isNumber = true; break;
            case BatteryType.Type_C: energyValue = 3f; symbolValue = "3"; isNumber = true; break;
            case BatteryType.Type_D: energyValue = 5f; symbolValue = "5"; isNumber = true; break;
            case BatteryType.Type_E: energyValue = 0f; symbolValue = "+"; isNumber = false; break;
            case BatteryType.Type_F: energyValue = 0f; symbolValue = "-"; isNumber = false; break;
            case BatteryType.Type_G: energyValue = 0f; symbolValue = "×"; isNumber = false; break;
            case BatteryType.Type_H: energyValue = 0f; symbolValue = "÷"; isNumber = false; break;
        }
    }

    public float GetEnergyValue() => energyValue;
    public string GetSymbolValue() => symbolValue;
    public bool IsNumberBattery() => isNumber;
}
