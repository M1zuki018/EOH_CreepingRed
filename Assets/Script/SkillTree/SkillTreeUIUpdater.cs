using UnityEngine.UI;

/// <summary>
/// スキルツリー画面のUI更新を補助するクラス
/// </summary>
public class SkillTreeUIUpdater : ISkillTreeUIUpdater
{
    // スキル説明エリア部分
    private readonly Text _skillName;
    private readonly Text _skillDescription;
    private readonly Text _point;
    private readonly Button _unlockButton;
    
    // フッター部分
    private Text _pointText; // 解放ポイント
    private readonly Slider _spreadSlider; // 拡散性スライダー
    private readonly Slider _detectionSlider; // 発覚率スライダー
    private readonly Slider _lethalitySlider; // 致死率スライダー

    public SkillTreeUIUpdater(
        Text skillName, Text skillDescription, Text point, Button unlockButton,
        Text pointText, Slider spreadSlider, Slider detectionSlider, Slider lethalitySlider)
    {
        _skillName = skillName;
        _skillDescription = skillDescription;
        _point = point;
        _unlockButton = unlockButton;
        _pointText = pointText;
        _spreadSlider = spreadSlider;
        _detectionSlider = detectionSlider;
        _lethalitySlider = lethalitySlider;
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
    /// スライダーの最大値の初期化
    /// </summary>
    public void InitializeSlider()
    {
        int maxValue = 110;
        _spreadSlider.maxValue = maxValue;
        _detectionSlider.maxValue = maxValue;
        _lethalitySlider.maxValue = maxValue;
    }
    
    /// <summary>
    /// 解放コスト/拡散性/発覚率/致死率のスライダーのUIを更新する
    /// </summary>
    public void UpdateUnderGauges()
    {
        //_pointText.text = Resource.ToString();
        _spreadSlider.value = InfectionParameters.BaseRate;
        //_detectionSlider.value = Detection;
        _lethalitySlider.value = InfectionParameters.LethalityRate;
    }
    
    /// <summary>
    /// スキルの解放ボタンにインタラクティブできるかどうかを切り替える
    /// </summary>
    public void ToggleUnlockButton(bool isUnlock)
    {
        _unlockButton.interactable = isUnlock;
    }
}
