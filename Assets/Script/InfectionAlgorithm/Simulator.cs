using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ViewBaseを継承したSimulationロジックの根幹となるクラス
/// </summary>
public class Simulator : ViewBase, ISimulator
{
    [SerializeField] private List<AreaSettingsSO> _areaSettings = new List<AreaSettingsSO>();
    private Simulation _simulation;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(ITimeObservable timeManager)
    {
        _simulation = new Simulation(_areaSettings, timeManager);
    }

#if UNITY_EDITOR
    /// <summary>
    /// エリアのスクリプタブルオブジェクトを登録する
    /// </summary>
    public void RegisterAreas(List<AreaSettingsSO> newAreas)
    {
        _areaSettings.Clear();
        _areaSettings.AddRange(newAreas);
    }
#endif
}
