using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: IAudioService.cs
/// 所屬模組: Audio
/// 責任描述: 定義 Awakening Protocol VR 之音訊播送與語音播放服務契約介面。
/// 架構分層: Audio / Interfaces
/// ============================================================================
/// </summary>
public interface IAudioService
{
    void PlaySFX(string key);
    AudioClip GetClip(string key);
    float GetVolume(string key);
}
