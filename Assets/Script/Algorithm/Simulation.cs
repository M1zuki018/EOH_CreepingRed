using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using R3;
using Debug = UnityEngine.Debug;

/// <summary>
/// 1日のサイクルを管理する
/// </summary>
public class Simulation : IDisposable
{
    private Grid _grid;
    private TimeManager _timeManager;
    private int day = 0;
    private int timeStep = 0;
    private int maxDays = 30;  // 1ヶ月

    public Simulation(List<AreaSettingsSO> areaSettings)
    {
        _grid = new Grid(areaSettings); // グリッドを生成する
        _timeManager = new TimeManager(); // 時間を管理するクラスを生成

        _timeManager.GameTimeProp.Subscribe(UpdateSimulation);
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

    private void HandleBattles()
    {
        // 魔法士と亡霊の戦闘ロジックを記述
    }

    public void Dispose()
    {
        _timeManager?.Dispose();
    }
}
