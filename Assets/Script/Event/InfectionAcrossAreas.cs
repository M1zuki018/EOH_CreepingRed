using System;
using R3;
using Random = System.Random;

/// <summary>
/// エリアをまたぐ感染を管理するクラス
/// </summary>
public class InfectionAcrossAreas : IDisposable
{
    private readonly Grid _grid; // Area情報を取得するためのGridクラスの参照
    private readonly int _rows; // Area二次元配列のヨコの長さ
    private readonly int _cols; // Area二次元配列のタテの長さ
    
    private readonly Random _random;
    private readonly IDisposable _spreadEventObserver;
    
    public InfectionAcrossAreas(Grid grid)
    {
        _grid = grid;
        _random = new Random();
        
        // 二次元配列の長さを取得
        _rows = _grid.Areas.GetLength(0);
        _cols = _grid.Areas.GetLength(1);
        
        //TODO: 10秒に一度感染を広げるチェックを行う(この条件をあとで変更すること)
        _spreadEventObserver = Observable
            .Interval(TimeSpan.FromSeconds(10))
            .Subscribe(_ => SpreadEvent());
    }
    
    /// <summary>
    /// 確率でエリアを跨いだ感染を発生させるクラス
    /// </summary>
    private void SpreadEvent()
    {
        int x = _random.Next(_rows);
        int y = _random.Next(_cols);
        
        _grid.Areas[x,y]?.Spread();
    }

    public void Dispose()
    {
        _spreadEventObserver?.Dispose();
    }
}
