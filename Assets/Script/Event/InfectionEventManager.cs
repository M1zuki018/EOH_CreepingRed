using System;
using R3;

/// <summary>
/// 感染イベント全体を管理するクラス
/// </summary>
public class InfectionEventManager : IDisposable
{
    private Infection_AcrossAreas _acrossAreas; // エリアを跨ぐ感染を管理するクラス
    private IDisposable _spreadEventObserver;
    
    public InfectionEventManager(Grid grid)
    {
        _acrossAreas = new Infection_AcrossAreas(grid);
        
        //TODO: 10秒に一度感染を広げるチェックを行う(この条件をあとで変更すること)
        _spreadEventObserver = Observable
            .Interval(TimeSpan.FromSeconds(10))
            .Subscribe(_ => _acrossAreas.SpreadEvent());
    }

    public void Dispose()
    {
        _spreadEventObserver?.Dispose();
    }
}
