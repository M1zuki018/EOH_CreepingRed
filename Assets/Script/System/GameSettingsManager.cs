using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// GameSettingsをラップする静的クラス
/// </summary>
public static class GameSettingsManager
{
    private static GameSettings _instance;

    private static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameSettings>("GameSettings");
                if (_instance == null)
                {
                    Debug.LogError("GameSettings.asset がResourcesフォルダ内にありません");
                }
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// カーソルの動く速さ
    /// </summary>
    public static float MouseSensitivity
    {
        get => Instance.MouseSensitivity;
        set => Instance.MouseSensitivity = value;
    }

    /// <summary>
    /// ズーム速度
    /// </summary>
    public static float ZoomSensitivity
    {
        get => Instance.ZoomSensitivity;
        set => Instance.ZoomSensitivity = value;
    }

    /// <summary>
    /// 全体の音量
    /// </summary>
    public static float MasterVolume
    {
        get => Instance.MasterVolume;
        set
        {
            Instance.MasterVolume = value;
            AudioMixerManager.SetVolume(AudioType.Master, Instance.MasterVolume);
        }
    }

    /// <summary>
    /// BGMの音量
    /// </summary>
    public static float BGMVolume
    {
        get => Instance.BGMVolume;
        set
        {
            Instance.BGMVolume = value;
            AudioMixerManager.SetVolume(AudioType.BGM, Instance.BGMVolume);
        }
    }

    /// <summary>
    /// SEの音量
    /// </summary>
    public static float SEVolume
    {
        get => Instance.SEVolume;
        set
        {
            Instance.SEVolume = value;
            AudioMixerManager.SetVolume(AudioType.SE, Instance.SEVolume);
        }
    }

    /// <summary>
    /// 環境音の音量
    /// </summary>
    public static float AmbientVolume
    {
        get => Instance.AmbientVolume;
        set
        {
            Instance.AmbientVolume = value;
            AudioMixerManager.SetVolume(AudioType.Ambience, Instance.AmbientVolume);
        }
    }

    /// <summary>
    /// ボイスの音量
    /// </summary>
    public static float VoiceVolume
    {
        get => Instance.VoiceVolume;
        set
        {
            Instance.VoiceVolume = value;
            AudioMixerManager.SetVolume(AudioType.Voice, Instance.VoiceVolume);
        }
    }
}
