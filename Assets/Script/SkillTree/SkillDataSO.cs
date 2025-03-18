using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各スキルのデータを管理するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "SkillData", menuName = "Create SO/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    [SerializeField, Comment("スキル名")] private string _name = "New Skill";
    [SerializeField, Comment("説明")] private string _description = "Skill Description";
    [SerializeField, Comment("解放コスト")] private int _cost = 1;
    [SerializeField, Comment("感染力")] private float _infectionRate = 0;
    [SerializeField, Comment("危険度")] private float _riskRate = 0;
    [SerializeField, Comment("致死率")] private float _resistanceRate = 0;
    [SerializeField, Comment("前提スキル")] private List<SkillButton> _prerequisiteSkills;
    
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public int Cost => _cost;
    public float InfectionRate => _infectionRate;
    public float RiskRate => _riskRate;
    public float ResistanceRate => _resistanceRate;
    public List<SkillButton> PrerequisiteSkills => _prerequisiteSkills;
    
    public void SetName(string newName) => _name = newName;
}
