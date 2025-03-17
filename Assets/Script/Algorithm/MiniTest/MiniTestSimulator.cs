using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 縮小シミュレーションテスト用のクラス
/// </summary>
public class MiniTestSimulator : ViewBase, ISimulator
{
    [SerializeField] private List<AreaSettingsSO> _areaSettings;
    
    private MiniSimulation _simulation;
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;
    
    public override UniTask OnAwake()
    {
        _timeManager = new TimeManager();
        // ヨコ5マス×タテ4マスのグリッド
        // 人口は9,130万人
        _simulation = new MiniSimulation(_areaSettings, _timeManager);
        
        return base.OnAwake();
    }
}
