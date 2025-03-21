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
[RequireComponent(typeof(CanvasGroup))]
public class TimerUIController : ViewBase, IWindow
{
    [SerializeField, HighlightIfNull, Comment("日付テキスト")] private Text _timeText;
    [SerializeField, HighlightIfNull, Comment("倍速ボタン")] private Button[] _timeScaleButtons = new Button[4];
    
    private CanvasGroup _canvasGroup;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    
        return base.OnUIInitialize();
    }

    public override UniTask OnBind()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        ITimeObservable timeManager = gameManager.TimeManager;
        new TimeView(_timeText, timeManager);
        new TimeScaleView(_timeScaleButtons, timeManager);
        
        return base.OnBind();
    }
    
    public void Show()
    {
        CanvasVisibilityController.Show(_canvasGroup);
    }
    
    public void Hide()
    {
        CanvasVisibilityController.Hide(_canvasGroup);
    }
    
    public void Block()
    {
        CanvasVisibilityController.Block(_canvasGroup);
    }
}
