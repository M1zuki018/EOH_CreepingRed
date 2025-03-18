using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各スキルのデータ
/// </summary>
[CreateAssetMenu(fileName = "SkillData", menuName = "Create SO/SkillData")]
public class SkillData : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    [SerializeField, Comment("スキル名")] private string _name;
    [SerializeField, Comment("説明")] private string _description;
    [SerializeField, Comment("解放コスト")] private int _cost;
    [SerializeField, Comment("感染力")] private float _infectionRate;
    [SerializeField, Comment("危険度")] private float _riskRate;
    [SerializeField, Comment("致死率")] private float _resistanceRate;
    [SerializeField, Comment("前提スキル")] private List<SkillButton> _prerequisiteSkills;
}
