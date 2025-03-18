using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 感染スキルのツリー
/// </summary>
public class ContagionSkillTree : ViewBase
{
    [SerializeField] private List<SkillButton> _skillButtons = new List<SkillButton>();

    public override UniTask OnBind()
    {
        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick += UIUpdate;
        }
        return base.OnBind();
    }

    /// <summary>
    /// 受け取ったスキルデータに合わせてUIを更新する
    /// </summary>
    private void UIUpdate(SkillDataSO skillData)
    {
        
    }
}
