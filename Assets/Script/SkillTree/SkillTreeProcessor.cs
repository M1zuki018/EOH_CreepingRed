using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキルツリーのロジックを担当するクラス
/// </summary>
public class SkillTreeProcessor
{
    private readonly Dictionary<SkillEnum, SkillButton> _skillButtonDic = new Dictionary<SkillEnum, SkillButton>();

    public SkillTreeProcessor(List<SkillButton> skillButtons)
    {
        // enumとボタンを対応させるDictionaryを作成
        foreach (var button in skillButtons)
        {
            if (Enum.TryParse(button.SkillData.name, out SkillEnum skillEnum)) _skillButtonDic[skillEnum] = button;
            else Debug.LogWarning($"無効なスキル名: {button.SkillData.name}");
        }
    }
    
    /// <summary>
    /// スキルがアンロック可能か判断する
    /// （アンロックの判定をする必要があるのでSkillButtonクラスが欲しい）
    /// </summary>
    public bool CanUnlockSkill(SkillButton button)
    {
        return !button.IsUnlocked && // 自身がアンロックされていない
               ArePrerequisiteSkillsUnlocked(button.SkillData.PrerequisiteSkillsEnum) && // 前提スキルが全て解除されている
               GameEventParameters.Resource.Value >= button.SkillData.Cost; // コストが足りている
    }

    /// <summary>
    /// 前提スキルがすべて解放されているか確かめる
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

    /// <summary>
    /// アンロックボタンが押されたときの処理
    /// (アンロックボタンが押されたときにはパラメーターの更新だけがしたいのでSOが欲しい)
    /// </summary>
    public void UnlockSkill(SkillDataSO data)
    {
        ApplySkillEffects(data);
        GameEventParameters.Resource.Value -= data.Cost; // コストを消費
    }

    /// <summary>
    /// スキルの効果を適用する
    /// </summary>
    private void ApplySkillEffects(SkillDataSO data)
    {
        InfectionParameters.BaseRate += data.SpreadRate; // 拡散性
        GameEventParameters.DetectionRate += (int) data.DetectionRate; // 発覚率（仮置き）
        InfectionParameters.LethalityRate += data.LethalityRate; // 致死率
        // TODO: その他のスキル効果もここに追加
    }
}
