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
    private SkillButton _currentSkillButton; // 押されているスキルボタンの情報を保持しておく
    private int _resource = 150;

    public override UniTask OnBind()
    {
        // 各ボタンのクリックイベントを購読
        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick += HandleClick;
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
    /// 受け取ったスキルデータに合わせてUIを更新する
    /// </summary>
    private void UIUpdate(SkillButton button)
    {
        _skillTreeUIController.SkillTextsUpdate(button.SkillData.Name, button.SkillData.Description,button.SkillData.Cost.ToString());
        
        if (_resource > button.SkillData.Cost)
        {
            _skillTreeUIController.ChangeUnlockButton(true); // コストが足りていたらActivateボタンをインタラクティブできるように設定
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
        _currentSkillButton.Unlock();
        _resource -= _currentSkillButton.SkillData.Cost; // 自分の解放ポイントを減らす
        InfectionParameters.BaseRate += _currentSkillButton.SkillData.SpreadRate; // 拡散性
        //TODO: 発覚率
        InfectionParameters.LethalityRate += _currentSkillButton.SkillData.LethalityRate; // 致死率
        //TODO: その他の効果についても
        Debug.Log($"拡散性{InfectionParameters.BaseRate}/ 致死率{InfectionParameters.LethalityRate}");
    }

    /// <summary>
    /// スキルツリーの参照を得る
    /// </summary>
    public override void SetUIController(SkillTreeUIController skillTreeUIController)
    {
        _skillTreeUIController = skillTreeUIController;
        Debug.Log(_skillTreeUIController);
    }
}
