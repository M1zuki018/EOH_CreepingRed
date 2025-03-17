using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using R3;
using Debug = UnityEngine.Debug;

/// <summary>
/// 縮小テスト用1日のサイクルを管理する
/// </summary>
public class MiniSimulation : IDisposable
{
    private MiniGrid _grid;
    private ITimeObservable _timeObserver;

    public MiniSimulation(List<AreaSettingsSO> areaSettings)
    {
        _grid = new MiniGrid(areaSettings); // グリッドを生成する
        _timeObserver = new TimeManager(); // 時間を管理するクラスを生成

        _timeObserver.GameTimeProp.Subscribe(UpdateSimulation);
    }
    
    /// <summary>
    /// 1更新のメソッド(等倍時には3秒に一回呼び出される)
    /// </summary>
    private void UpdateSimulation(int time)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Debug.Log($"ゲーム内時間: {time} 時間経過");
        _grid.SimulateInfectionAsync().Forget();
        stopwatch.Stop();
        Debug.Log($"更新完了 : 実行時間 {stopwatch.ElapsedMilliseconds} ミリ秒");
    }

    public void Dispose()
    {
        _timeObserver?.GameTimeProp.Dispose();
    }
}
