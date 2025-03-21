using System;

/// <summary>
/// SkillTreeUIUpdaterのインターフェース
/// </summary>
public interface ISkillTreeUIHandler
{
    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize();
    
    /// <summary>
    /// スキル表示のUIを更新する
    /// </summary>
    public void UpdateSkillInfo(string name, string description, string point);

    /// <summary>
    /// スライダーの最大値の初期化
    /// </summary>
    public void InitializeSliders();

    /// <summary>
    /// 解放コスト/拡散性/発覚率/致死率のスライダーのUIを更新する
    /// </summary>
    public void UpdatePrams();

    /// <summary>
    /// スキルの解放ボタンにインタラクティブできるかどうかを切り替える
    /// </summary>
    public void SetUnlockButtonState(bool isInteractable);
    
    public event Action OnUnlock;
}
