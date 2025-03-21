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
    [Header("InGameViewとしてのセットアップ")]
    [SerializeField, HighlightIfNull] private Button _closeButton;
    
    [Header("スキルツリーのためのセットアップ")]
    [SerializeField] private List<SkillTree> _skillTrees = new List<SkillTree>();
    [SerializeField, HighlightIfNull, Comment("スキル名のエリア")] private Text _skillName;
    [SerializeField, HighlightIfNull, Comment("スキル説明のエリア")] private Text _skillDescription;
    [SerializeField, HighlightIfNull, Comment("解放コストのエリア")] private Text _point;
    [SerializeField, HighlightIfNull, Comment("解放ボタン")] private Button _unlockButton;
    [SerializeField, HighlightIfNull, Comment("エゼキエルのスキルツリーボタン")] private Button _ezechielButton;
    
    [Header("スキルカテゴリの変更")]
    [SerializeField, HighlightIfNull, Comment("感染")] private Button _contagionButton;
    [SerializeField, HighlightIfNull, Comment("症状")] private Button _symptomsButton;
    [SerializeField, HighlightIfNull, Comment("能力")] private Button _abilityButton;

    [Header("フッター部分の参照")] 
    [SerializeField, HighlightIfNull, Comment("解放ポイント")] private Text _pointText;
    [SerializeField, HighlightIfNull, Comment("拡散性スライダー")] private Slider _spreadSlider;
    [SerializeField, HighlightIfNull, Comment("発覚率スライダー")] private Slider _detectionSlider;
    [SerializeField, HighlightIfNull, Comment("致死率スライダー")] private Slider _lethalitySlider;
    
    public event Action OnClose;
    public event Action OnShowEzechielTree;
    public event Action OnUnlock;

    public int Resource { get; set; } = 150; // 仮コスト
    public int Detection { get; set; } = 0; // 仮発覚率
    
    public override UniTask OnAwake()
    {
        // 各スキルツリークラスに自身の参照を渡す
        foreach (var skillTree in _skillTrees)
        {
            skillTree.SetUIController(this);
        }
        
        return base.OnAwake();
    }
    
    protected override void RegisterEvents()
    {
        _closeButton.onClick.AddListener(() => OnClose?.Invoke()); // 閉じる
        _ezechielButton.onClick.AddListener(() => OnShowEzechielTree?.Invoke()); //  エゼキエルのスキルツリーを開く
        _unlockButton.onClick.AddListener(() => OnUnlock?.Invoke()); // スキルアンロック
        _contagionButton.onClick.AddListener(() => ShowSkillTree(0));
        _symptomsButton.onClick.AddListener(() => ShowSkillTree(1));
        _abilityButton.onClick.AddListener(() => ShowSkillTree(2));
    }
    
    protected override void UnregisterEvents()
    {
        _closeButton.onClick.RemoveListener(() => OnClose?.Invoke()); // 閉じる
        _ezechielButton.onClick.RemoveListener(() => OnShowEzechielTree?.Invoke()); //  エゼキエルのスキルツリーを開く
        _unlockButton.onClick.RemoveListener(() => OnUnlock?.Invoke()); // スキルアンロック
        _contagionButton.onClick.RemoveListener(() => ShowSkillTree(0));
        _symptomsButton.onClick.RemoveListener(() => ShowSkillTree(1));
        _abilityButton.onClick.RemoveListener(() => ShowSkillTree(2));
    }

    public override UniTask OnBind()
    {
        // UI表示の初期化
        SkillTextsUpdate(" ", " ", " "); // 説明エリアの初期化
        InitializeSlider(); // SliderのMaxValueを変更
        UpdateUnderGauges(); // 下のバーの初期化
        ToggleUnlockButton(false); // UnlockButtonをインタラクティブできないように

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
            if (index == i)
            {
                _skillTrees[i].Show();
            }
            else
            {
                _skillTrees[i].Hide();
            }
        }
    }

    /// <summary>
    /// スライダーの最大値の初期化
    /// </summary>
    private void InitializeSlider()
    {
        int maxValue = 110;
        _spreadSlider.maxValue = maxValue;
        _detectionSlider.maxValue = maxValue;
        _lethalitySlider.maxValue = maxValue;
    }

    /// <summary>
    /// スキル表示のUIを更新する
    /// </summary>
    public void SkillTextsUpdate(string name, string description, string point)
    {
        _skillName.text = name;
        _skillDescription.text = description;
        _point.text = point;
    }

    /// <summary>
    /// 解放コスト/拡散性/発覚率/致死率のスライダーのUIを更新する
    /// </summary>
    public void UpdateUnderGauges()
    {
        _pointText.text = Resource.ToString();
        _spreadSlider.value = InfectionParameters.BaseRate;
        _detectionSlider.value = Detection;
        _lethalitySlider.value = InfectionParameters.LethalityRate;
    }

    /// <summary>
    /// スキルの解放ボタンにインタラクティブできるかどうかを切り替える
    /// </summary>
    public void ToggleUnlockButton(bool isUnlock)
    {
        _unlockButton.interactable = isUnlock;
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
