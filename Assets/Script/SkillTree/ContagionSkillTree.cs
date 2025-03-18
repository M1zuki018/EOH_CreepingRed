using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 感染スキルのツリー
/// </summary>
public class ContagionSkillTree : SkillBase
{
    [SerializeField] private List<SkillButton> _skillButtons = new List<SkillButton>();
    private SkillTreeUIController _skillTreeUIController;

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
    private void UIUpdate(SkillButton button)
    {
        _skillTreeUIController.SkillTextsUpdate(button.SkillData.Name, button.SkillData.Description,button.SkillData.Cost.ToString());
        _skillTreeUIController.ChangeUnlockButton(true);
    }

    /// <summary>
    /// スキルツリーの参照を得る
    /// </summary>
    public override void SetUIController(SkillTreeUIController skillTreeUIController)
    {
        _skillTreeUIController = skillTreeUIController;
    }
}
