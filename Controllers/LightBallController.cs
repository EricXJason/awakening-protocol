using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// ============================================================================
/// 檔案名稱: LightBallController.cs
/// 所屬模組: Controllers
/// 責任描述: 感覺動作期 (Sensorimotor Stage) 音階發光球體互動控制器。
/// 架構分層: Controllers / Interactable
/// 聯動關係:
///   - 依賴: LevelManager_SensorimotorStage, AudioManager, DOTween
/// ============================================================================
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class LightBallController : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private GameObject outlineObject;
    [SerializeField] private GameObject polygon;
    [SerializeField] private GameObject textObject;

    [Header("Pitch Ball ID (1~8, Do~DoHigh)")]
    [Range(1, 8)]
    [SerializeField] private int ballId = 1;

    [Header("Animation Settings")]
    [SerializeField] private float animationTime = 1.5f;
    [SerializeField] private float rotateSpeed = 15f;

    private Collider lightballCollider;
    private AudioSource audioSource;
    private string sfxKey;
    private Vector3 originalScale;
    private Tween scaleTween;

    private void Awake()
    {
        lightballCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;
        InitializeSFXKey();
    }

    private void Start()
    {
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;

        ResetLightBall();
        ShowLightBall();
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }

    private void Update()
    {
        if (polygon != null)
        {
            polygon.transform.Rotate(Vector3.up * (Time.deltaTime * rotateSpeed));
        }
    }

    private void InitializeSFXKey()
    {
        switch (ballId)
        {
            case 1: sfxKey = "Do"; break;
            case 2: sfxKey = "Re"; break;
            case 3: sfxKey = "Mi"; break;
            case 4: sfxKey = "Fa"; break;
            case 5: sfxKey = "Sol"; break;
            case 6: sfxKey = "La"; break;
            case 7: sfxKey = "Ti"; break;
            case 8: sfxKey = "DoHigh"; break;
            default: sfxKey = string.Empty; break;
        }
    }

    public event System.Action<LightBallController, int> OnBallTouched;

    public void OnLightBallTouched()
    {
        PlaySound();
        StartCoroutine(ShowTextOneShot(0.5f));

        if (outlineObject != null) outlineObject.SetActive(true);
        if (lightballCollider != null) lightballCollider.enabled = false;

        OnBallTouched?.Invoke(this, ballId);
    }

    public void ShowLightBall()
    {
        scaleTween?.Kill();
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        scaleTween = transform.DOScale(originalScale, animationTime).SetEase(Ease.InSine);
    }

    public void ResetLightBall()
    {
        scaleTween?.Kill();
        if (textObject != null) textObject.SetActive(false);
        if (outlineObject != null) outlineObject.SetActive(false);
        if (lightballCollider != null) lightballCollider.enabled = true;
    }

    private void PlaySound()
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(sfxKey))
        {
            var clip = AudioManager.Instance.GetClip(sfxKey);
            var vol = AudioManager.Instance.GetVolume(sfxKey);
            if (clip != null)
            {
                audioSource.PlayOneShot(clip, vol);
            }
        }
    }

    private IEnumerator DelayedResetRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetLightBall();
    }

    private IEnumerator ShowTextOneShot(float duration)
    {
        if (textObject != null) textObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        if (textObject != null && ballId != 8) textObject.SetActive(false);
    }
}
