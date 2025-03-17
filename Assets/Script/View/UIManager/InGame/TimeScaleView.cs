using UnityEngine.UI;

/// <summary>
/// ゲーム内時間の倍速を操作するボタンに対して、倍速を変更するメソッドを購読させる
/// </summary>
public class TimeScaleView
{
    public TimeScaleView(Button[] timeScaleButtons, ITimeObservable timeObservable)
    {
        var timeScaleButtons1 = timeScaleButtons;
        var timeObservable1 = timeObservable;
        
        timeScaleButtons1[0].onClick.AddListener(() => timeObservable1.SetTimeScale(0));
        timeScaleButtons1[1].onClick.AddListener(() => timeObservable1.SetTimeScale(1));
        timeScaleButtons1[2].onClick.AddListener(() => timeObservable1.SetTimeScale(2));
        timeScaleButtons1[3].onClick.AddListener(() => timeObservable1.SetTimeScale(3));
    }
}
