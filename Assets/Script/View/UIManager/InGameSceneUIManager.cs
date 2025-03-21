using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// InGameシーンのUI全体を管理するManager
/// </summary>
public class InGameSceneUIManager : UIManagerBase
{
    [Header("UIControllerの登録")]
    [SerializeField, HighlightIfNull, Comment("拠点画面")] private BaseViewUIController _baseView;
    [SerializeField, HighlightIfNull, Comment("全体ビュー")] private MacroViewUIController _macroView;
    [SerializeField, HighlightIfNull, Comment("区画ビュー")] private MicroViewUIController _microView;
    [SerializeField, HighlightIfNull, Comment("リタスキルツリー")] private SkillTreeUIController _skillTree;
    [SerializeField, HighlightIfNull, Comment("エゼキエルスキルツリー")] private EzechielSkillTreeUIController _ezechielSkillTree;
    [SerializeField, HighlightIfNull, Comment("タイマー")] private TimerUIController _timer;
    
    [Header("AreaViewSettingsの登録")]
    [SerializeField, HighlightIfNull] private List<AreaViewSettingsSO> _areaUISettings = new List<AreaViewSettingsSO>();

    public override UniTask OnBind()
    {
        _macroView.Initialize(_areaUISettings);
        _microView.Initialize(_areaUISettings);
        return base.OnBind();
    }

    protected override void RegisterEvents()
    {
        RegisterBaseViewEvents(); // 拠点画面
        RegisterMacroViewEvents(); // 全体画面
        RegisterMicroViewEvents(); // 区画画面
        RegisterSkillTreeEvents(); // スキルツリー
    }

    protected override void InitializePanel()
    {
        _baseView.Show();
        _timer.Show();
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
    
    protected override void UnregisterEvents()
    {
        _baseView.OnMacroView -= OnBaseViewMacroView;
        _macroView.OnSkillTree -= OnMacroViewSkillTree;
        _macroView.OnArea -= TransitionAreaView;
        _macroView.OnClose -= OnMacroViewClose;
        _microView.OnMacroView -= OnMicroViewMacroView;
        _skillTree.OnClose -= OnSkillTreeClose;
        _skillTree.OnShowEzechielTree -= OnSkillTreeShowEzechielTree;
        _ezechielSkillTree.OnClose -= OnEzechielSkillTreeClose;
        _ezechielSkillTree.OnShowRitaTree -= OnEzechielSkillTreeShowRitaTree;
    }
    
    #region Registerメソッド

    /// <summary>
    /// 拠点画面のイベント登録
    /// </summary>
    private void RegisterBaseViewEvents()
    {
        _baseView.OnMacroView += OnBaseViewMacroView;
    }
    
    /// <summary>
    /// 全体画面のイベント登録
    /// </summary>
    private void RegisterMacroViewEvents()
    {
        _macroView.OnSkillTree += OnMacroViewSkillTree;
        _macroView.OnArea += TransitionAreaView;
        _macroView.OnClose += OnMacroViewClose;
    }
    
    /// <summary>
    /// 区画画面のイベント登録
    /// </summary>
    private void RegisterMicroViewEvents()
    {
        _microView.OnMacroView += OnMicroViewMacroView;
    }
    
    /// <summary>
    /// スキルツリー画面のイベント登録
    /// </summary>
    private void RegisterSkillTreeEvents()
    {
        // リタのスキルツリー
        _skillTree.OnClose += OnSkillTreeClose;
        _skillTree.OnShowEzechielTree += OnSkillTreeShowEzechielTree;
        
        // エゼキエルのスキルツリー
        _ezechielSkillTree.OnClose += OnEzechielSkillTreeClose;
        _ezechielSkillTree.OnShowRitaTree += OnEzechielSkillTreeShowRitaTree;
    }

    #endregion
    
    #region イベントハンドラー

    private void OnBaseViewMacroView() => TransitionView(_macroView, _baseView);
    private void OnMacroViewSkillTree() => TransitionView(_skillTree, _macroView);
    private void OnMacroViewClose() => TransitionView(_baseView, _macroView);
    private void OnMicroViewMacroView() => TransitionView(_macroView, _microView);
    private void OnSkillTreeClose() => TransitionView(_macroView, _skillTree);
    private void OnSkillTreeShowEzechielTree() => TransitionView(_ezechielSkillTree, _skillTree);
    private void OnEzechielSkillTreeClose() => TransitionView(_macroView, _ezechielSkillTree);
    private void OnEzechielSkillTreeShowRitaTree() => TransitionView(_skillTree, _ezechielSkillTree);

    #endregion
}
