using System;

/// <summary>
/// SkillTreeUIUpdaterのインターフェース
/// </summary>
public interface ISkillTreeUIUpdater
{
    /// <summary>
    /// スキル表示のUIを更新する
    /// </summary>
    public void UpdateSkillInfo(string name, string description, string point);

    /// <summary>
    /// 解放コスト/拡散性/発覚率/致死率のスライダーのUIを更新する
    /// </summary>
    public void UpdateParameterSliders();

    /// <summary>
    /// スキルの解放ボタンにインタラクティブできるかどうかを切り替える
    /// </summary>
    public void SetUnlockButtonState(bool isInteractable);
    
    /// <summary>
    /// スキル解放が行われた際のイベント
    /// </summary>
    public event Action OnUnlock;
}
