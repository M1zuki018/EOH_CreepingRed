using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーンタイマーのUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class TimerUIController : UIControllerBase
{
    [SerializeField, HighlightIfNull, Comment("日付テキスト")] private Text _timeText;
    [SerializeField, HighlightIfNull, Comment("倍速ボタン")] private Button[] _timeScaleButtons = new Button[4];

    protected override void RegisterEvents() { } // 処理なし
    protected override void UnregisterEvents() { } // 処理なし

    public override UniTask OnBind()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            ITimeObservable timeManager = gameManager.TimeManager;
            new TimeView(_timeText, timeManager);
            new TimeScaleView(_timeScaleButtons, timeManager);
        }
        else
        {
            Debug.LogError($"{nameof(TimerUIController)} : GameManagerが見つかりませんでした");
        }
        
        return base.OnBind();
    }
    
    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
