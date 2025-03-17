using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// シミュレーションテスト用のクラス
/// </summary>
public class TestSimulator : ViewBase
{
    public List<AreaSettingsSO> AreaSettings = new List<AreaSettingsSO>();
    private Simulation _simulation;
    
    public override UniTask OnStart()
    {
        // ヨコ5マス×タテ4マスのグリッド
        // 人口は9,130万人
        _simulation = new Simulation(AreaSettings);
        
        return base.OnStart();
    }

    /// <summary>
    /// エリアのスクリプタブルオブジェクトを登録する
    /// </summary>
    public void RegisterAreas(List<AreaSettingsSO> newAreas)
    {
        AreaSettings.Clear();
        AreaSettings.AddRange(newAreas);
    }
}
