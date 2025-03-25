using UnityEngine;

/// <summary>
/// GameSettingsをラップする静的クラス
/// </summary>
public static class GameSettingsManager
{
    private static GameSettings _instance;
    
    private static DifficultyEnum _difficulty = DifficultyEnum.Breeze;
    private static int _startPointIndex = 7;
    
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
    /// 難易度
    /// </summary>
    public static DifficultyEnum Difficulty
    {
        get => _difficulty;
        set => _difficulty = value;
    }

    /// <summary>
    /// 感染開始地点のIndex
    /// </summary>
    public static int StartPointIndex
    {
        get => _startPointIndex;
        set => _startPointIndex = value;
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
