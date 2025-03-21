using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキルツリークラス
/// </summary>
public class SkillTree : UIControllerBase
{
    [SerializeField] private List<SkillButton> _skillButtons;
    public List<SkillButton> SkillButtons => _skillButtons;
    
    private readonly Dictionary<SkillEnum, SkillButton> _skillButtonDic = new Dictionary<SkillEnum, SkillButton>();
    private SkillButton _selectedSkillButton; // 押されているスキルボタンの情報を保持しておく
    private SkillTreeUIController _uiController; // UIControllerの参照
    
    protected override void RegisterEvents()
    {
        if (_uiController != null)
        {
            _uiController.OnUnlock += UnlockSkill;
        }
        else
        {
            Debug.LogError("SkillTree : UIController がありません");
        }

        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick += OnSkillButtonClick;　// 各ボタンのクリックイベントを購読
            
            if (Enum.TryParse(skillButton.SkillData.name, out SkillEnum skillEnum))
            {
                _skillButtonDic[skillEnum] = skillButton; // 辞書にスキル名のenumと対応するボタンをセットで登録
            }
            else
            {
                Debug.LogWarning($"無効なスキル名: {skillButton.SkillData.name}");
            }
        }
    }

    protected override void UnregisterEvents()
    {
        _uiController.OnUnlock -= UnlockSkill;
        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick -= OnSkillButtonClick;
        }
    }
    
    /// <summary>
    /// 各スキルのボタンが押された時の処理
    /// </summary>
    private void OnSkillButtonClick(SkillButton skill)
    {
        _selectedSkillButton = skill;
        UpdateSkillUI(skill); // UI更新
    }

    /// <summary>
    /// 前提スキルが全て解放されているか確かめる
    /// </summary>
    private bool ArePrerequisiteSkillsUnlocked(List<SkillEnum> prerequisiteSkills)
    {
        if (prerequisiteSkills == null) return true; // 前提スキルがなければtrueを返す
        
        foreach (var prerequisiteSkill in prerequisiteSkills)
        {
            if (!_skillButtonDic[prerequisiteSkill].IsUnlocked)
            {
                return false; // 未開放のスキルがあればその時点でfalseを返す
            }
        }
        return true; // 全て解放されていたらtrueを返す
    }
    
    /// <summary>
    /// 受け取ったスキルデータに合わせてUIを更新する
    /// </summary>
    private void UpdateSkillUI(SkillButton button)
    {
        _uiController.SkillTextsUpdate(button.SkillData.Name, button.SkillData.Description,button.SkillData.Cost.ToString());
        
        var canUnlock = CanUnlockSkill(button);
        _uiController.ToggleUnlockButton(canUnlock);
    }
    

    /// <summary>
    /// アンロックボタンが押されたときの処理
    /// </summary>
    private void UnlockSkill()
    {
        // 選択中のボタンがない もしくは 既にアンロック済みなら以降の処理を行わない（二度押し対策）
        if (_selectedSkillButton == null || _selectedSkillButton.IsUnlocked) return;
        
        _uiController.ToggleUnlockButton(false); // Activateボタンをインタラクティブ出来ないようにする
        _selectedSkillButton.Unlock();
        _uiController.Resource -= _selectedSkillButton.SkillData.Cost; // 自分の解放ポイントを減らす
        
        ApplySkillEffects(); // パラメーター変更

        _uiController.UpdateUnderGauges();
        Debug.Log($"スキル解放　現在の 拡散性{InfectionParameters.BaseRate}/ 致死率{InfectionParameters.LethalityRate}");
    }

    /// <summary>
    /// スキルによるパラメータ変更を適用
    /// </summary>
    private void ApplySkillEffects()
    {
        InfectionParameters.BaseRate += _selectedSkillButton.SkillData.SpreadRate; // 拡散性
        _uiController.Detection += (int) _selectedSkillButton.SkillData.DetectionRate; // 発覚率（仮置き）
        InfectionParameters.LethalityRate += _selectedSkillButton.SkillData.LethalityRate; // 致死率
        //TODO: その他の効果についても
    }

    /// <summary>
    /// スキルをアンロック可能か判断する
    /// </summary>
    private bool CanUnlockSkill(SkillButton button)
    {
        return !button.IsUnlocked && // 自身がアンロックされていない
               ArePrerequisiteSkillsUnlocked(button.SkillData.PrerequisiteSkillsEnum) && // 前提スキルが全て解除されている
               _uiController.Resource >= button.SkillData.Cost; // コストが足りている
    }
    
    /// <summary>
    /// スキルツリーの参照を受け取る
    /// </summary>
    public void SetUIController(SkillTreeUIController skillTreeUIController)
    {
        _uiController = skillTreeUIController;
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
