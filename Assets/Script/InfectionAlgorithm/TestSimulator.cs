using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// シミュレーションテスト用のクラス
/// </summary>
public class TestSimulator : ViewBase, ISimulator
{
    [SerializeField] private List<AreaSettingsSO> _areaSettings = new List<AreaSettingsSO>();
    public List<AreaSettingsSO> AreaSettings => _areaSettings;
    private Simulation _simulation;
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;

    public override UniTask OnAwake()
    {
        _timeManager = new TimeManager();
        // ヨコ5マス×タテ4マスのグリッド
        // 人口は9,130万人
        _simulation = new Simulation(_areaSettings, _timeManager);

        return base.OnAwake();
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
    public void Initialize(ITimeObservable timeManager)
    {
        throw new System.NotImplementedException();
    }
}
