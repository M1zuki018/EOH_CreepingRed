using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 感染スキルのツリー
/// </summary>
public class ContagionSkillTree : SkillBase
{
    [SerializeField] private List<SkillButton> _skillButtons = new List<SkillButton>();
    private readonly Dictionary<SkillEnum, SkillButton> _skillButtonDic = new Dictionary<SkillEnum, SkillButton>();
    private SkillTreeUIController _uiController;
    private SkillButton _selectedSkillButton; // 押されているスキルボタンの情報を保持しておく

    public override UniTask OnBind()
    {
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

        if (_uiController != null)
        {
            _uiController.OnUnlock += UnlockSkill;
        }
        
        return base.OnBind();
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
        
        bool canUnlock = !button.IsUnlocked && // 自身がアンロックされていない
                         ArePrerequisiteSkillsUnlocked(button.SkillData.PrerequisiteSkillsEnum) && // 前提スキルが全て解除されている
                         _uiController.Resource >= button.SkillData.Cost; // コストが足りている
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
        
        InfectionParameters.BaseRate += _selectedSkillButton.SkillData.SpreadRate; // 拡散性
        _uiController.Detection += (int) _selectedSkillButton.SkillData.DetectionRate; // 発覚率（仮置き）
        InfectionParameters.LethalityRate += _selectedSkillButton.SkillData.LethalityRate; // 致死率
        //TODO: その他の効果についても
        
        _uiController.UpdateUnderGauges();
        Debug.Log($"スキル解放　現在の 拡散性{InfectionParameters.BaseRate}/ 致死率{InfectionParameters.LethalityRate}");
    }

    /// <summary>
    /// スキルツリーの参照を得る
    /// </summary>
    public override void SetUIController(SkillTreeUIController skillTreeUIController)
    {
        _uiController = skillTreeUIController;
    }
}
