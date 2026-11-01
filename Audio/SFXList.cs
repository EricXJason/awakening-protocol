using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: SFXList.cs
/// 所屬模組: Audio
/// 責任描述: 集中儲存關卡語音與音效資源之 ScriptableObject 資料容器。
/// 架構分層: Audio / ScriptableObject Data
/// ============================================================================
/// </summary>
[CreateAssetMenu(fileName = "SFXList", menuName = "AwakeningProtocol/SFXList")]
public class SFXList : ScriptableObject
{
    [System.Serializable]
    public struct SFXData
    {
        public string key;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;
    }

    public SFXData[] SFXDataArray;

    private Dictionary<string, SFXData> sfxDictionary;

    private void BuildSFXDictionary()
    {
        sfxDictionary = new Dictionary<string, SFXData>();

        if (SFXDataArray == null) return;

        foreach (SFXData data in SFXDataArray)
        {
            if (string.IsNullOrEmpty(data.key)) continue;

            if (!sfxDictionary.ContainsKey(data.key))
            {
                sfxDictionary.Add(data.key, data);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[SFXList] Duplicate SFX key: {data.key}, skipped.");
#endif
            }
        }
    }

    public AudioClip GetClip(string key)
    {
        if (sfxDictionary == null)
            BuildSFXDictionary();

        if (sfxDictionary.TryGetValue(key, out SFXData data))
            return data.clip;

#if UNITY_EDITOR
        Debug.LogWarning($"[SFXList] SFX key not found: '{key}'");
#endif
        return null;
    }
}
