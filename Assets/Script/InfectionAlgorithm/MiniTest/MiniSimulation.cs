using System;
using System.Collections.Generic;
using R3;
using UnityEditor.Timeline.Actions;
using Debug = UnityEngine.Debug;

/// <summary>
/// 縮小テスト用のサイクルを管理する。更新を行う人
/// </summary>
public class MiniSimulation : IDisposable
{
    private readonly MiniGrid _grid;
    private readonly IDisposable _subscription;

    public MiniSimulation(List<AreaSettingsSO> areaSettings, ITimeObservable timeManager)
    {
        _grid = new MiniGrid(areaSettings); // グリッドを生成する
        _subscription = timeManager.GameTimeProp.Subscribe(UpdateSimulation);
    }

    public void Infection()
    {
        _grid.StartInfection();
    }
    
    /// <summary>
    /// 1更新のメソッド(等倍時には3秒に一回呼び出される)
    /// </summary>
    private async void UpdateSimulation(int time)
    {
        await _grid.SimulateInfectionAsync();
        Debug.Log($"\u23f1\ufe0fTimer： {time} 時間経過");
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}
