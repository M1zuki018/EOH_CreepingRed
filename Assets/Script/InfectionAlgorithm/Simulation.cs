using System;
using System.Collections.Generic;
using R3;

/// <summary>
/// 1日のサイクルを管理する
/// </summary>
public class Simulation : IDisposable
{
    private readonly Grid _grid;
    private readonly IDisposable _subscription;

    public Simulation(List<AreaSettingsSO> areaSettings, ITimeObservable timeManager)
    {
        _grid = new Grid(areaSettings); // グリッドを生成する
        _subscription = timeManager.GameTimeProp.Subscribe(UpdateSimulation);
    }
    
    /// <summary>
    /// 1更新のメソッド(等倍時には3秒に一回呼び出される)
    /// </summary>
    private async void UpdateSimulation(int time)
    {
        await _grid.SimulateInfectionAsync();
        DebugLogHelper.LogTestOnly($"\u23f1\ufe0fTimer： {time} 時間経過");
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}
