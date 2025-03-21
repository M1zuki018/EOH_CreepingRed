using System.Collections.Generic;
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
    [SerializeField, HighlightIfNull] private List<AreaViewSettingsSO> _areaUISettings = new List<AreaViewSettingsSO>();

    public override UniTask OnBind()
    {
        // タイマー系（日付・倍速）の初期化
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        ITimeObservable timeManager = gameManager.TimeManager;
        new TimeView(_timeText, timeManager);
        new TimeScaleView(_timeScaleButtons, timeManager);
        
        // 各ビューの初期化
        _macroView.Initialize(_areaUISettings);
        _microView.Initialize(_areaUISettings);
        
        return base.OnBind();
    }

    protected override void RegisterEvents()
    {
        // 拠点画面
        _baseView.OnMacroView += () => TransitionView(_macroView, _baseView);
        
        // 全体画面
        _macroView.OnSkillTree += () => TransitionView(_skillTree, _macroView);
        _macroView.OnArea += TransitionAreaView;
        _macroView.OnClose += () => TransitionView(_baseView, _macroView);
        
        // 区画画面
        _microView.OnMacroView += () => TransitionView(_macroView, _microView);
        
        // リタのスキルツリー
        _skillTree.OnClose += () => TransitionView(_macroView, _skillTree);
        _skillTree.OnShowEzechielTree += () => TransitionView(_ezechielSkillTree, _skillTree);
        
        // エゼキエルのスキルツリー
        _ezechielSkillTree.OnClose += () => TransitionView(_macroView, _ezechielSkillTree);
        _ezechielSkillTree.OnShowRitaTree += () => TransitionView(_skillTree, _ezechielSkillTree);
    }

    protected override void InitializePanel()
    {
        _baseView.Show();
        _macroView.Hide();
        _microView.Hide();
        _skillTree.Hide();
        _ezechielSkillTree.Hide();
    }

    /// <summary>
    /// 区域ビューへの遷移
    /// </summary>
    private void TransitionAreaView(int index)
    {
        _microView.ShowMicroView(index);
        _macroView.Hide();
    }
    
    /// <summary>
    /// エリアUIのスクリプタブルオブジェクトを登録する
    /// </summary>
    public void RegisterAreas(List<AreaViewSettingsSO> newAreas)
    {
        _areaUISettings.Clear();
        _areaUISettings.AddRange(newAreas);
    }
}
