using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーンのUI全体を管理するManager
/// </summary>
public class InGameSceneUIManager : UIManagerBase
{
    [SerializeField, HighlightIfNull] private BaseViewUIController _baseView;
    [SerializeField, HighlightIfNull] private MacroViewUIController _macroView;
    [SerializeField, HighlightIfNull] private MicroViewUIController _microView;
    [SerializeField, HighlightIfNull] private SkillTreeUIController _skillTree;
    [SerializeField, HighlightIfNull] private EzechielSkillTreeUIController _ezechielSkillTree;
    [SerializeField, HighlightIfNull] private TimerUIController _timer;
    
    [SerializeField, HighlightIfNull] private List<AreaViewSettingsSO> _areaUISettings = new List<AreaViewSettingsSO>();

    public override UniTask OnBind()
    {
        InitializeView();

        return base.OnBind();
    }

    protected override void RegisterEvents()
    {
        RegisterBaseViewEvents(); // 拠点画面
        RegisterMacroViewEvents(); // 全体画面
        RegisterMicroViewEvents(); // 区画画面
        RegisterSkillTreeEvents(); // スキルツリー
    }
    
    /// <summary>
    /// 画面の初期化処理
    /// </summary>
    private void InitializeView()
    {
        _macroView.Initialize(_areaUISettings);
        _microView.Initialize(_areaUISettings);
    }

    #region Registerメソッド

    /// <summary>
    /// 拠点画面のイベント登録
    /// </summary>
    private void RegisterBaseViewEvents()
    {
        _baseView.OnMacroView += () => TransitionView(_macroView, _baseView);
    }
    
    /// <summary>
    /// 全体画面のイベント登録
    /// </summary>
    private void RegisterMacroViewEvents()
    {
        _macroView.OnSkillTree += () => TransitionView(_skillTree, _macroView);
        _macroView.OnArea += TransitionAreaView;
        _macroView.OnClose += () => TransitionView(_baseView, _macroView);
    }
    
    /// <summary>
    /// 区画画面のイベント登録
    /// </summary>
    private void RegisterMicroViewEvents()
    {
        _microView.OnMacroView += () => TransitionView(_macroView, _microView);
    }
    
    /// <summary>
    /// スキルツリー画面のイベント登録
    /// </summary>
    private void RegisterSkillTreeEvents()
    {
        // リタのスキルツリー
        _skillTree.OnClose += () => TransitionView(_macroView, _skillTree);
        _skillTree.OnShowEzechielTree += () => TransitionView(_ezechielSkillTree, _skillTree);
        
        // エゼキエルのスキルツリー
        _ezechielSkillTree.OnClose += () => TransitionView(_macroView, _ezechielSkillTree);
        _ezechielSkillTree.OnShowRitaTree += () => TransitionView(_skillTree, _ezechielSkillTree);
    }

    #endregion

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
}
