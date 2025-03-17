using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// シミュレーションテスト用のクラス
/// </summary>
public class TestSimulator : ViewBase, ISimulator
{
    [SerializeField] private List<AreaSettingsSO> AreaSettings = new List<AreaSettingsSO>();
    private Simulation _simulation;
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;

    public override UniTask OnAwake()
    {
        _timeManager = new TimeManager();
        // ヨコ5マス×タテ4マスのグリッド
        // 人口は9,130万人
        _simulation = new Simulation(AreaSettings, _timeManager);

        return base.OnAwake();
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
