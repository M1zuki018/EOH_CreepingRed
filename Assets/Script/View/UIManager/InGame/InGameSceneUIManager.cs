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
    [SerializeField, HighlightIfNull] private MacroViewUIController _macroView;
    [SerializeField, HighlightIfNull] private MicroViewUIController _microView;
    
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
        _baseView.OnMacroView += () => TransitionView<IWindow>(_macroView, _baseView);
        
        return base.OnBind();
    }

    public override UniTask OnStart()
    {
        _baseView.Show();
        _macroView.Hide();
        _microView.Hide();
        
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
