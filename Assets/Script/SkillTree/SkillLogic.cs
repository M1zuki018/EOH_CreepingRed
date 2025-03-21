using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキルツリーのロジックを担当するクラス
/// </summary>
public class SkillLogic
{
    private Dictionary<SkillEnum, SkillButton> _skillButtonDic = new Dictionary<SkillEnum, SkillButton>();

    public SkillLogic(List<SkillButton> skillButtons)
    {
        // enumとボタンを対応させるDictionaryを作成
        foreach (var button in skillButtons)
        {
            if (Enum.TryParse(button.SkillData.name, out SkillEnum skillEnum))
            {
                _skillButtonDic[skillEnum] = button;
            }
            else
            {
                Debug.LogWarning($"無効なスキル名: {button.SkillData.name}");
            }
        }
    }

    /// <summary>
    /// アンロックボタンが押されたときの処理
    /// </summary>
    public void UnlockSkill(SkillButton button, SkillTreeUIController uiController)
    {
        ApplySkillEffects(button, uiController);
        uiController.Resource -= button.SkillData.Cost; // コストを消費
    }

    /// <summary>
    /// スキルによるパラメータ変更を適用
    /// </summary>
    private void ApplySkillEffects(SkillButton button, SkillTreeUIController uiController)
    {
        InfectionParameters.BaseRate += button.SkillData.SpreadRate; // 拡散性
        uiController.Detection += (int) button.SkillData.DetectionRate; // 発覚率（仮置き）
        InfectionParameters.LethalityRate += button.SkillData.LethalityRate; // 致死率
        // TODO: その他のスキル効果もここに追加
    }

    /// <summary>
    /// スキルがアンロック可能か判断する
    /// </summary>
    public bool CanUnlockSkill(SkillButton button, int availableResource)
    {
        return !button.IsUnlocked && // 自身がアンロックされていない
               ArePrerequisiteSkillsUnlocked(button.SkillData.PrerequisiteSkillsEnum) && // 前提スキルが全て解除されている
               availableResource >= button.SkillData.Cost; // コストが足りている
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
                return false;
            }
        }
        return true;
    }
}
