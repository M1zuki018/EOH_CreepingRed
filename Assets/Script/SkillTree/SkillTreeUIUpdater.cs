using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルツリー画面のUI更新を補助するクラス
/// </summary>
public class SkillTreeUIUpdater : ISkillTreeUIUpdater, IDisposable
{
    // スキル説明エリア部分
    private readonly Text _skillNameText;
    private readonly Text _skillDescriptionText;
    private readonly Text _unlockCostText;
    private readonly Button _unlockButton;
    
    // フッター部分
    private readonly Text _availablePointText; // 解放ポイント
    private readonly Slider _spreadSlider; // 拡散性スライダー
    private readonly Slider _detectionSlider; // 発覚率スライダー
    private readonly Slider _lethalitySlider; // 致死率スライダー
    
    public event Action OnUnlock;

    public SkillTreeUIUpdater(
        Text skillNameText, Text skillDescriptionText, Text unlockCostText, Button unlockButton,
        Text availablePointText, Slider spreadSlider, Slider detectionSlider, Slider lethalitySlider)
    {
        _skillNameText = skillNameText;
        _skillDescriptionText = skillDescriptionText;
        _unlockCostText = unlockCostText;
        _unlockButton = unlockButton;
        _availablePointText = availablePointText;
        _spreadSlider = spreadSlider;
        _detectionSlider = detectionSlider;
        _lethalitySlider = lethalitySlider;
        
        _unlockButton.onClick.AddListener(HandleUnlock); // スキルアンロック
        
        Initialize();
    }

    /// <summary>
    /// UI初期化処理
    /// </summary>
    private void Initialize()
    {
        InitializeSliders(); // SliderのMaxValueを変更
        UpdateSkillInfo(" ", " ", " "); // 説明エリアの初期化
        UpdateParameterSliders(); // 下のバーの初期化
        SetUnlockButtonState(false); // UnlockButtonをインタラクティブできないように
    }

    /// <summary>
    /// スライダーの最大値の初期化
    /// </summary>
    private void InitializeSliders()
    {
        int maxValue = 110;
        _spreadSlider.maxValue = maxValue;
        _detectionSlider.maxValue = maxValue;
        _lethalitySlider.maxValue = maxValue;
    }
    
    /// <summary>
    /// スキル表示のUIを更新する
    /// </summary>
    public void UpdateSkillInfo(string name, string description, string point)
    {
        _skillNameText.text = name;
        _skillDescriptionText.text = description;
        _unlockCostText.text = point;
    }
    
    /// <summary>
    /// 解放コスト/拡散性/発覚率/致死率のスライダーのUIを更新する
    /// </summary>
    public void UpdateParameterSliders()
    {
        _availablePointText.text = ParametersOtherThanInfectionLogic.Resource.ToString();
        _spreadSlider.value = InfectionParameters.BaseRate;
        _detectionSlider.value = ParametersOtherThanInfectionLogic.DetectionRate;
        _lethalitySlider.value = InfectionParameters.LethalityRate;
        
        Debug.Log($"スキル解放　現在の 拡散性{InfectionParameters.BaseRate}/ 致死率{InfectionParameters.LethalityRate}");
    }
    
    /// <summary>
    /// スキルの解放ボタンにインタラクティブできるかどうかを切り替える
    /// </summary>
    public void SetUnlockButtonState(bool isInteractable)
    {
        _unlockButton.interactable = isInteractable;
    }
    
    public void Dispose()
    {
        _unlockButton.onClick.RemoveListener(HandleUnlock);
    }
    
    private void HandleUnlock() => OnUnlock?.Invoke();
}
