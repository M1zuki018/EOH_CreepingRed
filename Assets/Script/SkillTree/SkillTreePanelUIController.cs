using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 各スキルツリーのUIを管理するクラス
/// </summary>
public class SkillTreePanelUIController : UIControllerBase
{
    [SerializeField] private List<SkillButton> _skillButtons;
    public List<SkillButton> SkillButtons => _skillButtons;
    
    private SkillButton _selectedSkillButton; // 押されているスキルボタンの情報を保持しておく
    private SkillTreeUIController _uiController; // UIControllerの参照
    private SkillLogic _logic; // 処理
    private ISkillTreeUIUpdater _skillTreeUIUpdater;

    public override UniTask OnAwake()
    {
        _logic = new SkillLogic(_skillButtons);
        return base.OnAwake();
    }
    
    protected override void RegisterEvents()
    {
        if (_uiController != null)
        {
            _uiController.OnUnlock += UnlockSkill;
        }
        else
        {
            Debug.LogError("SkillTree : UIController がありません");
        }

        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick += OnSkillButtonClick;　// 各ボタンのクリックイベントを購読
        }
    }

    protected override void UnregisterEvents()
    {
        _uiController.OnUnlock -= UnlockSkill;
        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick -= OnSkillButtonClick;
        }
    }
    
    /// <summary>
    /// 各スキルのボタンが押された時の処理
    /// </summary>
    private void OnSkillButtonClick(SkillButton skill)
    {
        _selectedSkillButton = skill;
        UpdateSkillUI(skill); // UI更新
    }
    
    /// <summary>
    /// 受け取ったスキルデータに合わせてUIを更新する
    /// </summary>
    private void UpdateSkillUI(SkillButton button)
    {
        _skillTreeUIUpdater.SkillTextsUpdate(button.SkillData.Name, button.SkillData.Description,button.SkillData.Cost.ToString());
        _skillTreeUIUpdater.ToggleUnlockButton(_logic.CanUnlockSkill(button, _uiController.Resource));
    }
    
    /// <summary>
    /// アンロックボタンが押されたときの処理
    /// </summary>
    private void UnlockSkill()
    {
        // 選択中のボタンがない もしくは 既にアンロック済みなら以降の処理を行わない（二度押し対策）
        if (_selectedSkillButton == null || _selectedSkillButton.IsUnlocked) return;
        
        _logic.UnlockSkill(_selectedSkillButton, _uiController);
        _skillTreeUIUpdater.ToggleUnlockButton(false); // Activateボタンをインタラクティブ出来ないようにする
        _skillTreeUIUpdater.UpdateUnderGauges();
        
        Debug.Log($"スキル解放　現在の 拡散性{InfectionParameters.BaseRate}/ 致死率{InfectionParameters.LethalityRate}");
    }
    
    /// <summary>
    /// スキルツリーの参照を受け取る
    /// </summary>
    public void SetUIController(SkillTreeUIController skillTreeUIController)
    {
        _uiController = skillTreeUIController;
        _skillTreeUIUpdater = skillTreeUIController.UIUpdater;
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
