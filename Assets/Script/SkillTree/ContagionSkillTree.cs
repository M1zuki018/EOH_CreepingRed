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
    private Dictionary<SkillEnum, SkillButton> _skillButtonDictionary = new Dictionary<SkillEnum, SkillButton>();
    private SkillTreeUIController _skillTreeUIController;
    private SkillButton _currentSkillButton; // 押されているスキルボタンの情報を保持しておく

    public override UniTask OnBind()
    {
        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick += HandleClick;　// 各ボタンのクリックイベントを購読
            
            if (Enum.TryParse(skillButton.SkillData.name, out SkillEnum skillEnum))
            {
                _skillButtonDictionary[skillEnum] = skillButton; // 辞書にスキル名のenumと対応するボタンをセットで登録
            }
            else
            {
                Debug.LogWarning($"Enumに変換できないスキル名: {skillButton.SkillData.name}");
            }
        }

        if (_skillTreeUIController != null)
        {
            _skillTreeUIController.OnUnlock += HandleUnlock;
        }
        
        return base.OnBind();
    }
    
    /// <summary>
    /// 各スキルのボタンが押された時の処理
    /// </summary>
    private void HandleClick(SkillButton skill)
    {
        UIUpdate(skill); // UI更新
        _currentSkillButton = skill;
    }

    /// <summary>
    /// 前提スキルが全て解放されているか確かめる
    /// </summary>
    private bool CheckPrerequisiteSkill(List<SkillEnum> prerequisiteSkills)
    {
        foreach (var prerequisiteSkill in prerequisiteSkills)
        {
            if (!_skillButtonDictionary[prerequisiteSkill].IsUnlocked)
            {
                return false; // 未開放のスキルがあればその時点でfalseを返す
            }
        }
        return true; // 全て解放されていたらtrueを返す
    }
    
    /// <summary>
    /// 受け取ったスキルデータに合わせてUIを更新する
    /// </summary>
    private void UIUpdate(SkillButton button)
    {
        _skillTreeUIController.SkillTextsUpdate(button.SkillData.Name, button.SkillData.Description,button.SkillData.Cost.ToString());
        
        if (!button.IsUnlocked && // 自身がアンロックされていない
            CheckPrerequisiteSkill(button.SkillData.PrerequisiteSkillsEnum) && // 前提スキルが全て解除されている
            _skillTreeUIController.Resource > button.SkillData.Cost) // コストが足りている
        {
            _skillTreeUIController.ChangeUnlockButton(true); // Activateボタンをインタラクティブできるように設定
        }
        else
        {
            _skillTreeUIController.ChangeUnlockButton(false);
        }
    }
    
    /// <summary>
    /// アンロックボタンが押されたときの処理
    /// </summary>
    private void HandleUnlock()
    {
        if(_currentSkillButton.IsUnlocked) return; // 既にアンロック済みなら以降の処理を行わない（二度押し対策）
        
        _skillTreeUIController.ChangeUnlockButton(false); // Activateボタンをインタラクティブ出来ないようにする
        
        _currentSkillButton.Unlock();
        _skillTreeUIController.Resource -= _currentSkillButton.SkillData.Cost; // 自分の解放ポイントを減らす
        InfectionParameters.BaseRate += _currentSkillButton.SkillData.SpreadRate; // 拡散性
        _skillTreeUIController.Detection += (int) _currentSkillButton.SkillData.DetectionRate; // 発覚率（仮置き）
        InfectionParameters.LethalityRate += _currentSkillButton.SkillData.LethalityRate; // 致死率
        //TODO: その他の効果についても
        
        _skillTreeUIController.UpdateUnderGauges();
        Debug.Log($"スキル解放　現在の 拡散性{InfectionParameters.BaseRate}/ 致死率{InfectionParameters.LethalityRate}");
    }

    /// <summary>
    /// スキルツリーの参照を得る
    /// </summary>
    public override void SetUIController(SkillTreeUIController skillTreeUIController)
    {
        _skillTreeUIController = skillTreeUIController;
    }
}
