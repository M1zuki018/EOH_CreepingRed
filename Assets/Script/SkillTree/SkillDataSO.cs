using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField, Comment("拡散性")] private float _spreadRate = 0; 
    [SerializeField, Comment("発覚率")] private float _detectionRate = 0;
    [SerializeField, Comment("致死率")] private float _lethalityRate = 0;
    [SerializeField, Comment("前提スキル")] private List<SkillEnum> _prerequisiteSkillsEnum;
    
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public int Cost => _cost;
    public float SpreadRate => _spreadRate;
    public float DetectionRate => _detectionRate;
    public float LethalityRate => _lethalityRate;
    public List<SkillEnum> PrerequisiteSkillsEnum => _prerequisiteSkillsEnum;

    /// <summary>
    /// データをセットする
    /// </summary>
    public void SetData(string name, string description, int cost, float infectionRate, float riskRate, float resistanseRate)
    {
        _name = name;
        _description = description;
        _cost = cost;
        _spreadRate = infectionRate;
        _detectionRate = riskRate;
        _lethalityRate = resistanseRate;
    }
}
