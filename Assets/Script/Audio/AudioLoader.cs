using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resourcesフォルダ内からクリップデータを自動でロードする機能
/// </summary>
public static class AudioLoader
{
    /// <summary>
    /// 指定したEnum型のオーディオクリップをロードする
    /// </summary>
    public static Dictionary<T, AudioClip> LoadAudioClips<T>(string folderPath) where T : Enum
    {
        Dictionary<T, AudioClip> dictionary = new Dictionary<T, AudioClip>();

        foreach (var item in Enum.GetValues(typeof(T)))
        {
            string path = $"{folderPath}/{item}";
            AudioClip clip = Resources.Load<AudioClip>(path);
            if (clip != null)
            {
                dictionary[(T)item] = clip;
            }
            else
            {
                Debug.LogError($"オーディオクリップが見つかりませんでした。パス: {path}");
            }
        }

        return dictionary;
    }
}
