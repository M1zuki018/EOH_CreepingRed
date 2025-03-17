using R3;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内時間経過を表示するクラス
/// </summary>
public class TimeView
{
    private Text _timeText;

    public TimeView(Text timeText)
    {
        _timeText = timeText;
        if (_timeText == null)
        {
            Debug.LogError("TimeビュークラスのTimeTextがnullです");
        }
    }
    
    public void Subscribe(ITimeObservable timeManager)
    {
        timeManager.GameTimeProp.Subscribe(_ => UpdateTimeView());
    }

    private void UpdateTimeView()
    {
        
    }
}
