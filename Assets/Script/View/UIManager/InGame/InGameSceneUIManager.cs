using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーンのUI全体を管理するManager
/// </summary>
public class InGameSceneUIManager : UIManagerBase
{
    [SerializeField, HighlightIfNull, Comment("日付テキスト")] private Text _timeText;
    [SerializeField, HighlightIfNull, Comment("倍速ボタン")] private Button[] _timeScaleButtons = new Button[4];
    [SerializeField, HighlightIfNull] private BaseViewUIController _baseView;
    
    public override UniTask OnAwake()
    {
        // Canvasコンポーネントの設定
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        
        return base.OnAwake();
    }

    public override UniTask OnBind()
    {
        ITimeObservable timeManager = FindAnyObjectByType<GameManager>().TimeManager;
        new TimeView(_timeText, timeManager);
        new TimeScaleView(_timeScaleButtons, timeManager);
        _baseView.OnMacroView += () => ShowMacroView();
        
        return base.OnBind();
    }

    private void ShowMacroView()
    {
        
    }

    public override UniTask OnStart()
    {
        _baseView.Show();
        
        return base.OnStart();
    }
    
    /// <summary>
    /// シーン遷移
    /// </summary>
    private void TransitionScene()
    {
        TransitionScene("Dev_Result");
    }
}
