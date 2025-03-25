using UnityEngine;

/// <summary>
/// Debug.Logの拡張クラス
/// </summary>
public static class DebugLogHelper
{
    public static bool IsObjectCreationLoggingEnabled { get; set; } = true;
    public static bool IsTestLoggingEnabled { get; set; } = true;
    
    /// <summary>
    /// 目立つログを出力する
    /// </summary>
    public static void LogImportant(object message)
    {
        string border = new string('=', 20); // 長いラインを作る
        Debug.Log($"<color=red><b>{border}{message}{border}</b></color>");
    }
    
    /// <summary>
    /// シーン基盤Object生成時に使用しているフォーマット付きのログ出力
    /// </summary>
    public static void LogObjectCreation(string format, params object[] args)
    {
        if (IsObjectCreationLoggingEnabled)
        {
            Debug.LogFormat(format, args);
        }
    }

    /// <summary>
    /// テスト中のみ表示したいログ
    /// </summary>
    public static void LogTestOnly(object message)
    {
        if (IsTestLoggingEnabled)
        {
            Debug.Log(message);
        }
    }
}
