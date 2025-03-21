using UnityEngine;

/// <summary>
/// Debug.Logの拡張クラス
/// </summary>
public static class DebugLogHelper
{
    public static bool IsLogFormatEnabled { get; set; } = true;
    public static bool IsLoggingEnabled { get; set; } = true;
    
    /// <summary>
    /// 目立つログを出力する
    /// </summary>
    public static void LogImportant(string message)
    {
        string border = new string('=', 20); // 長いラインを作る
        Debug.Log($"<color=red><b>{border}{message}{border}</b></color>");
    }
    
    /// <summary>
    /// フォーマット付きのログ出力
    /// </summary>
    public static void LogFormat(string format, params object[] args)
    {
        if (IsLogFormatEnabled)
        {
            Debug.LogFormat(format, args);
        }
    }

    /// <summary>
    /// テスト中のみ表示したいログ
    /// </summary>
    public static void TestOnly(string message)
    {
        if (IsLoggingEnabled)
        {
            Debug.Log(message);
        }
    }
}
