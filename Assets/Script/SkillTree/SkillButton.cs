using UnityEngine;

/// <summary>
/// スキルボタン用のクラス
/// </summary>
public class SkillButton　: ViewBase
{
    [SerializeField, HighlightIfNull, Expandable] private SkillData _skillData;
}
