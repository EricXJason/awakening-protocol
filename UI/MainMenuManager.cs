using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ============================================================================
/// 檔案名稱: MainMenuManager.cs
/// 所屬模組: UI
/// 責任描述: 負責首頁語系選擇 (繁中/英文) 以及啟動非同步進入遊戲關卡。
/// 架構分層: UI / Views
/// ============================================================================
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button chineseButton;
    [SerializeField] private Button englishButton;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerStatus(false);
        }

        if (chineseButton != null) chineseButton.onClick.AddListener(OnChineseButtonClicked);
        if (englishButton != null) englishButton.onClick.AddListener(OnEnglishButtonClicked);
    }

    private void OnDisable()
    {
        if (chineseButton != null) chineseButton.onClick.RemoveListener(OnChineseButtonClicked);
        if (englishButton != null) englishButton.onClick.RemoveListener(OnEnglishButtonClicked);
    }

    private void OnChineseButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentLanguage(GameLanguage.Chinese);
        }
        LoadNext();
    }

    private void OnEnglishButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentLanguage(GameLanguage.English);
        }
        LoadNext();
    }

    private void LoadNext()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadNextScene();
        }
    }
}
