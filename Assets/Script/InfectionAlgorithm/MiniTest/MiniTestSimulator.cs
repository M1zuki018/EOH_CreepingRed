using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 縮小シミュレーションテスト用のクラス
/// </summary>
public class MiniTestSimulator : ViewBase, ISimulator
{
    [SerializeField] private List<AreaSettingsSO> _areaSettings = new List<AreaSettingsSO>();
    private MiniSimulation _simulation;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(ITimeObservable timeManager)
    {
        _simulation = new MiniSimulation(_areaSettings, timeManager);
    }
}
