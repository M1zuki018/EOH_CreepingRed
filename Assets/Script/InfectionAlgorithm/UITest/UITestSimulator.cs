using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UITest用のSimulatorクラス
/// </summary>
public class UITestSimulator : ViewBase, ISimulator
{
    private Simulation _simulation;
    [SerializeField, HighlightIfNull] private List<AreaSettingsSO> _areaSettings;
    public List<AreaSettingsSO> AreaSettings => _areaSettings;
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;

    public override UniTask OnAwake()
    {
        _timeManager = new TimeManager();

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
