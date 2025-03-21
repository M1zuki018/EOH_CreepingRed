using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField, Range(0,1)] private float _bgmVolume = 1.0f;
    [SerializeField, Range(0,1)] private float _seVolume = 1.0f;
    [SerializeField, Range(0,1)] private float _ambientVolume = 1.0f;
    [SerializeField, Range(0,1)] private float _voiceVolume = 1.0f;
    [FormerlySerializedAs("_screenResolution")] [SerializeField] private ScreenResolutionEnum screenResolutionEnum = ScreenResolutionEnum._1920_1080; // 画面解像度を変更
    [SerializeField] private bool _fpsLimit; // フレームレートの上限
    
    /// <summary>
    /// PlayerPrefsのセットとゲットを統一するための汎用メソッド
    /// </summary>
    private float GetFloat(string key, float defaultValue) =>
        PlayerPrefs.GetFloat(key, defaultValue);

    private void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// カーソルの動く速さ
    /// </summary>
    public float MouseSensitivity
    {
        get => GetFloat("MouseSensitivity", _mouseSensitivity);
        set => SetFloat("MouseSensitivity", value);
    }

    /// <summary>
    /// ズーム速度
    /// </summary>
    public float ZoomSensitivity
    {
        get => GetFloat("ZoomSensitivity", _zoomSensitivity);
        set => SetFloat("ZoomSensitivity", value);
    }

    /// <summary>
    /// 全体の音量
    /// </summary>
    public float MasterVolume
    {
        get => GetFloat("MasterVolume", _masterVolume);
        set => SetFloat("MasterVolume", value);
    }

    /// <summary>
    /// BGMの音量
    /// </summary>
    public float BGMVolume
    {
        get => GetFloat("BGMVolume", _bgmVolume);
        set => SetFloat("BGMVolume", value);
    }

    /// <summary>
    /// SEの音量
    /// </summary>
    public float SEVolume
    {
        get => GetFloat("SEVolume", _seVolume);
        set => SetFloat("SEVolume", value);
    }

    /// <summary>
    /// 環境音の音量
    /// </summary>
    public float AmbientVolume
    {
        get => GetFloat("AmbientVolume", _ambientVolume);
        set => SetFloat("AmbientVolume", value);
    }

    /// <summary>
    /// ボイスの音量
    /// </summary>
    public float VoiceVolume
    {
        get => GetFloat("VoiceVolume", _voiceVolume);
        set => SetFloat("VoiceVolume", value);
    }
}
