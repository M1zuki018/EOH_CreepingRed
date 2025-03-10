using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioMixerを管理する静的クラス
/// </summary>
public static class AudioMixerManager
{
    private static AudioMixer _mixer;

    /// <summary>
    /// AudioMixerの参照をセットする
    /// </summary>
    public static void SetupMixer(AudioMixer mixer)
    {
        _mixer = mixer;
    }
    
    /// <summary>
    /// ゲーム内音量設定を受けとってAudioMixerのVolumeをセットする
    /// パラメーター名は"BGMVolume"という感じになる
    /// </summary>
    public static void SetVolume(AudioType type, float volume)
    {
        if (_mixer == null)
        {
            Debug.LogWarning("AudioMixerManager AudioMixerの参照がありません！");
            return;
        }
        
        if (volume == 0)
        {
            // 0なら完全にミュートする
            _mixer.SetFloat(type + "Volume", -80);
        }
        else
        {
            float volumeInDb = Mathf.Log10(volume) * 20; // ゲーム内音量設定を0~1の範囲で受け取ってdBに変換
            _mixer.SetFloat(type + "Volume", volumeInDb);
            _mixer.GetFloat(type + "Volume", out volume);
            Debug.Log(type.ToString() + " : " + volume);
        }
    }
}
