using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 縮小シミュレーションテスト用のクラス
/// ViewBaseを継承して、AreaSettingsSOのSerializeFieldを設定できるようにする
/// </summary>
public class MiniTestSimulator : ViewBase, ISimulator
{
    [SerializeField, ExpandableSO] private List<AreaSettingsSO> _areaSettings = new List<AreaSettingsSO>();
    private MiniSimulation _simulation;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(ITimeObservable timeManager)
    {
        _simulation = new MiniSimulation(_areaSettings, timeManager);
    }
    
    [MethodButtonInspector]
    public void Infection()
    {
        _simulation.Infection();
    }
}
