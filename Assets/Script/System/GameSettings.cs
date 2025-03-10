using UnityEngine;

/// <summary>
/// プレイヤーのゲーム設定
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Create SO/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private bool _mouseHoverHelp = true; // マウスをかざすと説明を表示するか
    [SerializeField, Range(0,1)] private float _mouseSensitivity = 1.0f;
    [SerializeField, Range(0,1)] private float _zoomSensitivity = 1.0f;
    [SerializeField, Range(0,1)] private float _masterVolume = 1.0f;
    [SerializeField, Range(0,1)] private float _bgmVolume = 1.0f; // BGMの音量
    [SerializeField, Range(0,1)] private float _seVolume = 1.0f; // 効果音の音量
    [SerializeField, Range(0,1)] private float _ambientVolume = 1.0f; // 環境音の音量
    [SerializeField, Range(0,1)] private float _voiceVolume = 1.0f; // ボイスの音量
    [SerializeField] private ScreenResolution _screenResolution = ScreenResolution._1920_1080; // 画面解像度を変更
    [SerializeField] private bool _fpsLimit; // フレームレートの上限

    /// <summary>
    /// カーソルの動く速さ
    /// </summary>
    public float MouseSensitivity
    {
        get => PlayerPrefs.GetFloat("MouseSensitivity", _mouseSensitivity);
        set
        {
            PlayerPrefs.SetFloat("MouseSensitivity", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// ズーム速度
    /// </summary>
    public float ZoomSensitivity
    {
        get => PlayerPrefs.GetFloat("ZoomSensitivity", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("ZoomSensitivity", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 全体の音量
    /// </summary>
    public float MasterVolume
    {
        get => PlayerPrefs.GetFloat("MasterVolume", _masterVolume);
        set
        {
            PlayerPrefs.SetFloat("MasterVolume", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// BGMの音量
    /// </summary>
    public float BGMVolume
    {
        get => PlayerPrefs.GetFloat("BGMVolume", _bgmVolume);
        set
        {
            PlayerPrefs.SetFloat("BGMVolume", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// SEの音量
    /// </summary>
    public float SEVolume
    {
        get => PlayerPrefs.GetFloat("SEVolume", _seVolume);
        set
        {
            PlayerPrefs.SetFloat("SEVolume", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 環境音の音量
    /// </summary>
    public float AmbientVolume
    {
        get => PlayerPrefs.GetFloat("AmbientVolume", _ambientVolume);
        set
        {
            PlayerPrefs.SetFloat("AmbientVolume", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// ボイスの音量
    /// </summary>
    public float VoiceVolume
    {
        get => PlayerPrefs.GetFloat("VoiceVolume", _voiceVolume);
        set
        {
            PlayerPrefs.SetFloat("VoiceVolume", value);
            PlayerPrefs.Save();
        }
    }
}
