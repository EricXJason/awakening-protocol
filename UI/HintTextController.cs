using TMPro;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: HintTextController.cs
/// 所屬模組: UI
/// 責任描述: 雙語解謎提示標籤，依據 GameManager.CurrentLanguage 自動切換字串。
/// 架構分層: UI / Views
/// ============================================================================
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class HintTextController : MonoBehaviour
{
    [SerializeField] private string hintSubtitleCH;
    [SerializeField] private string hintSubtitleEN;

    private TextMeshProUGUI hintText;

    private void Awake()
    {
        hintText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (hintText == null) return;

        bool isChinese = GameManager.Instance == null || GameManager.Instance.CurrentLanguage == GameLanguage.Chinese;
        hintText.text = isChinese ? hintSubtitleCH : hintSubtitleEN;
    }
}
