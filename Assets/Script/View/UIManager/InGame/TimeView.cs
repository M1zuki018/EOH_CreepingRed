using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内時間経過を表示するクラス
/// </summary>
public class TimeView : IDisposable
{
    private Text _timeText;
    private ITimeObservable _timeObservable;

    public TimeView(Text timeText, ITimeObservable timeObservable)
    {
        _timeText = timeText;
        _timeObservable = timeObservable;
        
        _timeObservable.GameTimeProp.Subscribe(_ => UpdateTimeView()); // 時間更新のPropの購読を開始
    }
    
    /// <summary>
    /// 時間表示のUIを書き換える
    /// </summary>
    private void UpdateTimeView()
    {
        Debug.Log("Time");
    }

    public void Dispose()
    {
        _timeObservable.GameTimeProp?.Dispose();
    }
}
