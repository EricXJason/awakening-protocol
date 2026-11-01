using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: AudioManager.cs
/// 所屬模組: Audio
/// 責任描述: 全域常駐音效與語音播放服務。實作 IAudioService 契約。
/// 架構分層: Audio / Manager
/// 聯動關係:
///   - 介面實作: IAudioService
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景根節點下的空物件，命名為「[AudioManager]」。
/// 2. 生命週期: 腳本內建 DontDestroyOnLoad，保持全域常駐。
/// ============================================================================
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour, IAudioService
{
    private static AudioManager instance;

    [Header("SFX Configuration Lists")]
    [SerializeField] private SFXList[] sfxLists;

    private AudioSource audioSource;
    private Dictionary<string, SFXList.SFXData> sfxDictionary;

    public static AudioManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        BuildSFXDictionary();
    }

    private void BuildSFXDictionary()
    {
        sfxDictionary = new Dictionary<string, SFXList.SFXData>();

        if (sfxLists == null) return;

        foreach (var list in sfxLists)
        {
            if (list == null || list.SFXDataArray == null) continue;

            foreach (var data in list.SFXDataArray)
            {
                if (string.IsNullOrEmpty(data.key)) continue;

                if (!sfxDictionary.ContainsKey(data.key))
                {
                    sfxDictionary.Add(data.key, data);
                }
            }
        }

#if UNITY_EDITOR
        Debug.Log($"[AudioManager] Audio dictionary built with {sfxDictionary.Count} clips loaded.");
#endif
    }

    public void PlaySFX(string key)
    {
        if (sfxDictionary == null || !sfxDictionary.TryGetValue(key, out var data))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[AudioManager] SFX key not found: '{key}'.");
#endif
            return;
        }

        if (data.clip != null)
        {
            audioSource.PlayOneShot(data.clip, data.volume);
        }
    }

    public AudioClip GetClip(string key)
    {
        if (sfxDictionary != null && sfxDictionary.TryGetValue(key, out var data))
        {
            return data.clip;
        }
        return null;
    }

    public float GetVolume(string key)
    {
        if (sfxDictionary != null && sfxDictionary.TryGetValue(key, out var data))
        {
            return data.volume;
        }
        return 1f;
    }
}
