using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

/// <summary>
/// Stopwatchクラスの使用を補助する静的クラス
/// </summary>
public static class StopwatchHelper
{
    /// <summary>
    /// 同期処理の実行時間を計測しテスト中のみログに出力する
    /// </summary>
    public static void Measure(Action action, string label = "処理時間")
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        action.Invoke();
        stopwatch.Stop();
        DebugLogHelper.LogTestOnly($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
    
    /// <summary>
    /// 非同期処理の実行時間を計測しテスト中のみログに出力する
    /// </summary>
    public static async UniTask MeasureAsync(Func<UniTask> action, string label = "処理時間")
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await action();
        stopwatch.Stop();
        DebugLogHelper.LogTestOnly($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
    
    /// <summary>
    /// フラグ設定に関係なく常に表示されるログ(同期)
    /// </summary>
    public static void AlwaysUse(Action action, string label = "処理時間")
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        action.Invoke();
        stopwatch.Stop();
        Debug.Log($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
    
    /// <summary>
    /// フラグ設定に関係なく常に表示されるログ(非同期)
    /// </summary>
    public static async UniTask AlwaysUseAsync(Func<UniTask> action, string label = "処理時間")
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await action();
        stopwatch.Stop();
        Debug.Log($"{label}: {stopwatch.ElapsedMilliseconds}ミリ秒");
    }
}
