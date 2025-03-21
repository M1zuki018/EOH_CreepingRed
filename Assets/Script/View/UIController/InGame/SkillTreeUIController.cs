using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーンスキルツリー画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class SkillTreeUIController : UIControllerBase
{
    #region フィールド

    [Header("InGameViewとしてのセットアップ")]
    [SerializeField, HighlightIfNull] private Button _closeButton;
    [SerializeField, HighlightIfNull, Comment("エゼキエルのスキルツリーボタン")] private Button _ezechielTreeButton;
    
    [Header("スキルツリーのためのセットアップ")]
    [SerializeField] private List<SkillTreePanelUIController> _skillTrees = new List<SkillTreePanelUIController>();
    [SerializeField, HighlightIfNull, Comment("スキル名のエリア")] private Text _skillNameText;
    [SerializeField, HighlightIfNull, Comment("スキル説明のエリア")] private Text _skillDescriptionText;
    [SerializeField, HighlightIfNull, Comment("解放コストのエリア")] private Text _unlockCostText;
    [SerializeField, HighlightIfNull, Comment("解放ボタン")] private Button _unlockButton;
    
    [Header("スキルカテゴリの変更")]
    [SerializeField, HighlightIfNull, Comment("感染")] private Button _contagionButton;
    [SerializeField, HighlightIfNull, Comment("症状")] private Button _symptomsButton;
    [SerializeField, HighlightIfNull, Comment("能力")] private Button _abilityButton;

    [Header("フッター部分の参照")] 
    [SerializeField, HighlightIfNull, Comment("解放ポイント")] private Text _availablePointText;
    [SerializeField, HighlightIfNull, Comment("拡散性スライダー")] private Slider _spreadSlider;
    [SerializeField, HighlightIfNull, Comment("発覚率スライダー")] private Slider _detectionSlider;
    [SerializeField, HighlightIfNull, Comment("致死率スライダー")] private Slider _lethalitySlider;
    
    public event Action OnClose;
    public event Action OnShowEzechielTree;

    #endregion
    
    public override UniTask OnAwake()
    {
        var uiHandler = new SkillTreeUIUpdater(
            _skillNameText, _skillDescriptionText, _unlockCostText, _unlockButton,
            _availablePointText, _spreadSlider, _detectionSlider, _lethalitySlider);
        
        // 各スキルツリークラスにUIHandlerの参照を渡す
        foreach (var skillTree in _skillTrees)
        {
            skillTree.SetSkillTreeUIHandler(uiHandler);
        }
        
        return base.OnAwake();
    }
    
    protected override void RegisterEvents()
    {
        _closeButton.onClick.AddListener(HandleCloseClick); // 閉じる
        _ezechielTreeButton.onClick.AddListener(HandleShowEzechielTreeClick); //  エゼキエルのスキルツリーを開く
        _contagionButton.onClick.AddListener(ShowContagionTree);
        _symptomsButton.onClick.AddListener(ShowSymptomsTree);
        _abilityButton.onClick.AddListener(ShowAbilityTree);
    }
    
    protected override void UnregisterEvents()
    {
        _closeButton.onClick.RemoveListener(HandleCloseClick); // 閉じる
        _ezechielTreeButton.onClick.RemoveListener(HandleShowEzechielTreeClick); //  エゼキエルのスキルツリーを開く
        _contagionButton.onClick.RemoveListener(ShowContagionTree);
        _symptomsButton.onClick.RemoveListener(ShowSymptomsTree);
        _abilityButton.onClick.RemoveListener(ShowAbilityTree);
    }

    public override UniTask OnBind()
    {
        // スキルツリーパネルの操作
        ShowSkillTree(0);
        
        return base.OnBind();
    }
    
    /// <summary>
    /// 指定されたIndexのスキルツリーパネルを表示する
    /// </summary>
    private void ShowSkillTree(int index)
    {
        for (int i = 0; i < _skillTrees.Count; i++)
        {
            if (index == i) _skillTrees[i].Show();
            else　_skillTrees[i].Hide();
        }
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
    
    private void HandleCloseClick() => OnClose?.Invoke();
    private void HandleShowEzechielTreeClick() => OnShowEzechielTree?.Invoke();
    private void ShowContagionTree() => ShowSkillTree(0);
    private void ShowSymptomsTree() => ShowSkillTree(1);
    private void ShowAbilityTree() => ShowSkillTree(2);
}
