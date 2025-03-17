using R3;

/// <summary>
/// 時間の更新を通知するためのインターフェース
/// </summary>
public interface ITimeObservable
{
    /// <summary>
    /// 倍速（1倍、2倍、3倍）
    /// </summary>
    ReactiveProperty<float> TimeScaleProp { get; } 
    /// <summary>
    /// ゲーム内時間（例: 経過時間を時間単位で管理）
    /// </summary>
    ReactiveProperty<int> GameTimeProp { get; } 
}
