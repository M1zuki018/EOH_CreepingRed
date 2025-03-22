using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
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
        DebugLogHelper.TestOnly($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
    
    public static async UniTask MeasureAsync(Func<UniTask> action, string label = "処理時間")
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await action();
        stopwatch.Stop();
        DebugLogHelper.TestOnly($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
    
    /// <summary>
    /// フラグ設定に関係なく常に表示されるログ
    /// </summary>
    public static void AlwaysUse(Action action, string label = "処理時間")
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        action.Invoke();
        stopwatch.Stop();
        Debug.Log($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
}
