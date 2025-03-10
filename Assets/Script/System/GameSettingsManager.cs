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
}
