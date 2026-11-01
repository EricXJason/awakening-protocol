using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// ============================================================================
/// 檔案名稱: PauseMenuController.cs
/// 所屬模組: UI
/// 責任描述: VR 暫停選單控制器。支援 Input System 動作綁定、全域遮罩淡入淡出、
///           回首頁與退出遊戲功能。
/// 架構分層: UI / Views
/// ============================================================================
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("Input Action Bindings")]
    [SerializeField] private InputActionReference toggleMenuAction;

    [Header("UI Components")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Text homeButtonText;
    [SerializeField] private Text quitButtonText;

    [Header("Animation and Audio")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private string sfxKey = "Menu";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private bool isMenuOpen = false;

    private void Awake()
    {
        if (pauseMenuCanvas != null)
        {
            canvasGroup = pauseMenuCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = pauseMenuCanvas.AddComponent<CanvasGroup>();
            pauseMenuCanvas.SetActive(false);
        }
    }

    private void Start()
    {
        if (toggleMenuAction != null && toggleMenuAction.action != null)
        {
            toggleMenuAction.action.started += OnToggleMenuPressed;
            toggleMenuAction.action.Enable();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPauseMenuRequested += OpenMenu;
        }

        if (homeButton != null) homeButton.onClick.AddListener(OnHomeClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPauseMenuRequested -= OpenMenu;
        }

        if (toggleMenuAction != null && toggleMenuAction.action != null)
        {
            toggleMenuAction.action.started -= OnToggleMenuPressed;
        }
    }

    private void OnToggleMenuPressed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlayable) return;

        isMenuOpen = !isMenuOpen;
        if (isMenuOpen) OpenMenu();
        else CloseMenu();
    }

    private void OpenMenu()
    {
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerStatus(false);
            GameManager.Instance.TriggerGlobalDimmer(0.85f, fadeDuration);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxKey);
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMenuRoutine(1f, true));
    }

    private void CloseMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerStatus(true);
            GameManager.Instance.TriggerGlobalDimmer(0f, fadeDuration);
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMenuRoutine(0f, false));
    }

    private IEnumerator FadeMenuRoutine(float targetAlpha, bool isInteractable)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        canvasGroup.interactable = isInteractable;
        canvasGroup.blocksRaycasts = isInteractable;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (!isInteractable && pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    private void OnHomeClicked()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneByName(mainMenuSceneName);
        }
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        Debug.Log("[PauseMenu] Application quit requested.");
#else
        Application.Quit();
#endif
    }
}
