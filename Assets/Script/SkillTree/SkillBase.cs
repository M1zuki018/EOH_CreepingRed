using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキルツリークラスが継承するインターフェース
/// </summary>
public abstract class SkillBase : ViewBase
{
    [SerializeField] private List<SkillButton> _skillButtons;
    public List<SkillButton> SkillButtons => _skillButtons;
    public abstract void SetUIController(SkillTreeUIController skillTreeUIController);
}
