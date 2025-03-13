using UnityEngine;

/// <summary>
/// Debug.Logの独自追加機能
/// </summary>
public static class DebugLogHelper
{
    /// <summary>
    /// 目立つログを出力する
    /// </summary>
    public static void LogImportant(string message)
    {
        string border = new string('=', 20); // 長いラインを作る
        Debug.Log($"<color=red><b>{border}{message}{border}</b></color>");
    }
    
    
}
