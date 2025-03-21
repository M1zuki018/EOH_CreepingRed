using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 各スキルツリーのUIを管理するクラス
/// </summary>
public class SkillTreePanelUIController : UIControllerBase
{
    [SerializeField] private List<SkillButton> _skillButtons; // 自分の子のスキルボタン
    private SkillButton _selectedSkillButton; // 押されているスキルボタンの情報を保持しておく
    private SkillLogic _logic; // スキルの解放処理を担当
    private ISkillTreeUIHandler _skillTreeUIHandler; // スキルツリー画面全体のUI更新処理を担当

    public override UniTask OnAwake()
    {
        _logic = new SkillLogic(_skillButtons);
        return base.OnAwake();
    }
    
    protected override void RegisterEvents()
    {
        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick += HandleSkillButtonClick;　// 各ボタンのクリックイベントを購読
        }
    }

    protected override void UnregisterEvents()
    {
        _skillTreeUIHandler.OnUnlock -= HandleUnlockSkill;
        foreach (SkillButton skillButton in _skillButtons)
        {
            skillButton.OnClick -= HandleSkillButtonClick;
        }
    }
    
    /// <summary>
    /// 各スキルのボタンが押された時の処理
    /// </summary>
    private void HandleSkillButtonClick(SkillButton button)
    {
        _selectedSkillButton = button;
        UpdateSkillUI();
    }
    
    /// <summary>
    /// スキルUIを更新するようにUIHandlerに指示を出す(スキル獲得前)
    /// </summary>
    private void UpdateSkillUI()
    {
        if (_selectedSkillButton == null) return;

        var data = _selectedSkillButton.SkillData;
        _skillTreeUIHandler.UpdateSkillInfo(data.Name, data.Description, data.Cost.ToString());
        _skillTreeUIHandler.SetUnlockButtonState(_logic.CanUnlockSkill(_selectedSkillButton));
    }

    /// <summary>
    /// アンロックボタンが押されたときの処理
    /// </summary>
    private void HandleUnlockSkill()
    {
        // 選択中のボタンがない もしくは 既にアンロック済みなら以降の処理を行わない（二度押し対策）
        if (_selectedSkillButton == null || _selectedSkillButton.IsUnlocked) return;
        
        _logic.UnlockSkill(_selectedSkillButton.SkillData);
        _skillTreeUIHandler.SetUnlockButtonState(false); // Activateボタンをインタラクティブ出来ないようにする
        _skillTreeUIHandler.UpdatePrams(); // スライダーUIを更新する
    }

    /// <summary>
    /// スキルツリーのUIHandlerの参照を得る
    /// </summary>
    public void SetSkillTreeUIHandler(ISkillTreeUIHandler skillTreeUIHandler)
    {
        _skillTreeUIHandler = skillTreeUIHandler;
        _skillTreeUIHandler.OnUnlock += HandleUnlockSkill;
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
    
#if UNITY_EDITOR
    /// <summary>
    /// エディタ専用のスキルボタン追加メソッド（実行時には使用不可）
    /// </summary>
    public void AddSkillButton(SkillButton skillButton)
    {
        if (!_skillButtons.Contains(skillButton))
        {
            _skillButtons.Add(skillButton);
        }
    }
#endif
}
