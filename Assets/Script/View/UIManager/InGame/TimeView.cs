using System;
using DG.Tweening;
using R3;
using UnityEngine.UI;

/// <summary>
/// ゲーム内時間経過を表示するクラス
/// </summary>
public class TimeView : IDisposable
{
    private readonly Text _timeText;
    private readonly ITimeObservable _timeObservable;
    private DateTime _currentTime;

    public TimeView(Text timeText, ITimeObservable timeObservable)
    {
        _timeText = timeText;
        _timeObservable = timeObservable;
        
        _currentTime = new DateTime(2100, 08, 23, 14, 00, 00); // 初期の日付 2100-08-23 14:00:00
        
        _timeObservable.GameTimeProp.Subscribe(_ => UpdateTimeView()); // 時間更新のPropの購読を開始
    }
    
    /// <summary>
    /// 時間表示のUIを書き換える
    /// フェードでぼんやり切り替わるようなアニメーションつき
    /// </summary>
    private void UpdateTimeView()
    {
        _currentTime = _currentTime.AddHours(2); // _currentTimeを更新
        _timeText.DOFade(0.5f, 0.4f)
            .OnComplete(() =>
            {
                _timeText.text = _currentTime.ToString("yyyy-MM-dd\nHH:mm"); // フォーマットを整えて書き換え
                _timeText.DOFade(1, 0.4f);
            });
    }

    public void Dispose()
    {
        _timeObservable.GameTimeProp?.Dispose();
    }
}
