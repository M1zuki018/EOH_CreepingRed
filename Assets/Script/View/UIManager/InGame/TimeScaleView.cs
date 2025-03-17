using UnityEngine.UI;

/// <summary>
/// ゲーム内時間の倍速を操作するボタンのView
/// </summary>
public class TimeScaleView
{
    private Button[] _timeScaleButtons;
    private ITimeObservable _timeObservable;
    
    public TimeScaleView(Button[] timeScaleButtons, ITimeObservable timeObservable)
    {
        _timeScaleButtons = timeScaleButtons;
        _timeObservable = timeObservable;
        
        _timeScaleButtons[0].onClick.AddListener(() => _timeObservable.SetTimeScale(0));
        _timeScaleButtons[1].onClick.AddListener(() => _timeObservable.SetTimeScale(1));
        _timeScaleButtons[2].onClick.AddListener(() => _timeObservable.SetTimeScale(2));
        _timeScaleButtons[3].onClick.AddListener(() => _timeObservable.SetTimeScale(3));
    }
}
