using R3;

/// <summary>
/// 時間の更新を通知するためのインターフェース
/// </summary>
public interface ITimeObservable
{
    /// <summary>
    /// ゲーム内時間（例: 経過時間を時間単位で管理）
    /// </summary>
    ReadOnlyReactiveProperty<int> GameTimeProp { get; } 
    
    /// <summary>
    /// 倍速の設定を変更する
    /// </summary>
    void SetTimeScale(float scale){}
}
