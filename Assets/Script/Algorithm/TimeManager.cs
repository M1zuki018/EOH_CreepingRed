using System;
using R3;

/// <summary>
/// ゲーム内時間を管理するクラス
/// </summary>
public class TimeManager : IDisposable
{
    private ReactiveProperty<float> _timeScaleProp = new ReactiveProperty<float>(1f); // 倍速（1倍、2倍、3倍）
    public ReactiveProperty<int> GameTimeProp { get; } = new ReactiveProperty<int>(0); // ゲーム内時間（例: 経過時間を時間単位で管理）
    
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
        _timeSubscription = Observable
            .Interval(TimeSpan.FromSeconds(3f / _timeScaleProp.Value))
            .Subscribe(_ => GameTimeProp.Value += 2);
    }

    /// <summary>
    /// 倍速の設定を変更する
    /// </summary>
    public void SetTimeScale(float scale)
    {
        if (scale <= 0 || scale > 3) return; // 無効な倍速を防ぐ
        _timeScaleProp.Value = scale;
        SubscribeToTimeUpdates(); // 新しい倍速で購読を再設定する
    }

    public void Dispose()
    {
        _timeSubscription?.Dispose();
    }
}