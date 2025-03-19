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
    [SerializeField, HighlightIfNull] private SkillTreeUIController _skillTree;
    [SerializeField, HighlightIfNull] private EzechielSkillTreeUIController _ezechielSkillTree;
    
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
        // タイマー系（日付・倍速）の初期化
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        ITimeObservable timeManager = gameManager.TimeManager;
        new TimeView(_timeText, timeManager);
        new TimeScaleView(_timeScaleButtons, timeManager);
        
        // 都市全体ビューの初期化
        _macroView.Initialize(gameManager.AreaSettings);
        
        // イベント登録
        _baseView.OnMacroView += () => TransitionView<IWindow>(_macroView, _baseView);
        
        _macroView.OnSkillTree += () => TransitionView<IWindow>(_skillTree, _macroView);
        _macroView.OnArea += () => TransitionView<IWindow>(_microView, _macroView);
        _macroView.OnClose += () => TransitionView<IWindow>(_baseView, _macroView);
        
        _skillTree.OnClose += () => TransitionView<IWindow>(_macroView, _skillTree);
        _skillTree.OnShowEzechielTree += () => TransitionView<IWindow>(_ezechielSkillTree, _skillTree);
        
        _ezechielSkillTree.OnShowRitaTree += () => TransitionView<IWindow>(_skillTree, _ezechielSkillTree);
        
        _microView.OnMacroView += () => TransitionView<IWindow>(_macroView, _microView);
        
        return base.OnBind();
    }

    public override UniTask OnStart()
    {
        _baseView.Show();
        _macroView.Hide();
        _microView.Hide();
        _skillTree.Hide();
        _ezechielSkillTree.Hide();
        
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
