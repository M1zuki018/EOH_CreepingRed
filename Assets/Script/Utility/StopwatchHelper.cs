using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// Stopwatchクラスの使用を補助する静的クラス
/// </summary>
public static class StopwatchHelper
{
    public static void Measure(Action action, string label = "処理時間")
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        action.Invoke();
        stopwatch.Stop();
        Debug.Log($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
}
