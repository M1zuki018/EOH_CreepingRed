/// <summary>
/// SkillTreeUIUpdaterのインターフェース
/// </summary>
public interface ISkillTreeUIUpdater
{
    /// <summary>
    /// スキル表示のUIを更新する
    /// </summary>
    public void SkillTextsUpdate(string name, string description, string point){}
    
    /// <summary>
    /// スライダーの最大値の初期化
    /// </summary>
    public void InitializeSlider(){}
    
    /// <summary>
    /// 解放コスト/拡散性/発覚率/致死率のスライダーのUIを更新する
    /// </summary>
    public void UpdateUnderGauges(){}
}
