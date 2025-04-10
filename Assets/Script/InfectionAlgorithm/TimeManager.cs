using System;
using R3;

/// <summary>
/// ゲーム内時間を管理するクラス
/// </summary>
public class TimeManager : ITimeObservable, IDisposable
{
    private readonly ReactiveProperty<float> _timeScaleProp = new ReactiveProperty<float>(1); // 倍速
    private readonly ReactiveProperty<int> _gameTimeProp = new ReactiveProperty<int>(0); // ゲーム内時間
    
    public ReadOnlyReactiveProperty<int> GameTimeProp => _gameTimeProp;
    
    private IDisposable _timeSubscription;

    public TimeManager()
    {
        SubscribeToTimeUpdates();
    }

    /// <summary>
    /// 時間計測の購読
    /// </summary>
    private void SubscribeToTimeUpdates()
    {
        _timeSubscription?.Dispose(); // 既存の購読を解除する
        _timeSubscription = _timeScaleProp
            .Where(value => value > 0) // timeScaleProp.Value が 0 より大きいときのみ実行
            .Select(value => Observable.Interval(TimeSpan.FromSeconds(3f / value)))
            .Switch() // 既存の Interval をキャンセルし、新しい Interval を開始
            .Subscribe(_ => _gameTimeProp.Value += 2);
    }

    /// <summary>
    /// 倍速の設定を変更する
    /// </summary>
    public void SetTimeScale(float scale)
    {
        if (scale < 0 || scale > 3) return; // 無効な倍速を防ぐ。有効範囲は0~3
        _timeScaleProp.Value = scale;
        SubscribeToTimeUpdates(); // 新しい倍速で購読を再設定する
    }

    public void Dispose()
    {
        _timeSubscription?.Dispose();
    }
}