using System;
using R3;

/// <summary>
/// スキルを解放するためのコストを取得するイベント
/// </summary>
public class GetCost : IDisposable
{
    private readonly IDisposable _costSubscription; // 一定時間おきにコストを取得するObservable
    
    public GetCost()
    {
        _costSubscription = Observable
            .Interval(TimeSpan.FromSeconds(5))
            .Subscribe(_ => Get());
    }
    
    /// <summary>
    /// リソースを取得する
    /// </summary>
    private void Get()
    {
        GameEventParameters.Resource.Value += 5;
    }

    public void Dispose()
    {
        _costSubscription?.Dispose();
    }
}
