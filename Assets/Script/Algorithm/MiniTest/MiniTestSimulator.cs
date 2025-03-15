using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 縮小シミュレーションテスト用のクラス
/// </summary>
public class MiniTestSimulator : ViewBase
{
    [SerializeField] private List<AreaSettingsSO> _areaSettings;
    private MiniSimulation _simulation;
    
    public override UniTask OnStart()
    {
        // ヨコ5マス×タテ4マスのグリッド
        // 人口は9,130万人
        _simulation = new MiniSimulation(_areaSettings);
        
        return base.OnStart();
    }
}
