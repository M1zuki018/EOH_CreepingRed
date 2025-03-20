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
[RequireComponent(typeof(CanvasGroup))]
public class SkillTreeUIController : ViewBase, IWindow
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

    [Header("フッター部分の参照")] 
    [SerializeField, HighlightIfNull, Comment("解放ポイント")] private Text _pointText;
    [SerializeField, HighlightIfNull, Comment("拡散性スライダー")] private Slider _spreadSlider;
    [SerializeField, HighlightIfNull, Comment("発覚率スライダー")] private Slider _detectionSlider;
    [SerializeField, HighlightIfNull, Comment("致死率スライダー")] private Slider _lethalitySlider;
    
    private CanvasGroup _canvasGroup;
    public event Action OnClose;
    public event Action OnShowEzechielTree;
    public event Action OnUnlock;

    public int Resource { get; set; } = 150; // 仮コスト
    public int Detection { get; set; } = 0; // 仮発覚率
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        // 各ボタンにイベントを登録
        _closeButton.onClick.AddListener(() => OnClose?.Invoke()); // 閉じる
        _ezechielButton.onClick.AddListener(() => OnShowEzechielTree?.Invoke()); //  エゼキエルのスキルツリーを開く
        _unlockButton.onClick.AddListener(() => OnUnlock?.Invoke()); // スキルアンロック

        // 各スキルツリークラスに自身の参照を渡す
        foreach (var skillTree in _skillTrees)
        {
            skillTree.SetUIController(this);
        }
        
        // UI表示の初期化
        SkillTextsUpdate(" ", " ", " "); // 説明エリアの初期化
        InitializeSlider(); // SliderのMaxValueを変更
        UpdateUnderGauges(); // 下のバーの初期化
        ToggleUnlockButton(false); // UnlockButtonをインタラクティブできないように
        
        return base.OnUIInitialize();
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

    private void OnDestroy()
    {
        // 登録解除
        _unlockButton.onClick.RemoveAllListeners(); 
        _closeButton.onClick.RemoveAllListeners();
        _ezechielButton.onClick.RemoveAllListeners();
    }
}
