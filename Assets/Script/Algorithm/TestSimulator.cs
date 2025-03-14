using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// シミュレーションテスト用のクラス
/// </summary>
public class TestSimulator : ViewBase
{
    [SerializeField] private List<AreaSettingsSO> _areaSettings;
    private Simulation _simulation;
    
    public override UniTask OnStart()
    {
        // ヨコ5マス×タテ4マスのグリッド
        // 人口は9,130万人
        _simulation = new Simulation(_areaSettings);
        
        return base.OnStart();
    }

    public void Update()
    {
        //_simulation.Run();
    }
}
