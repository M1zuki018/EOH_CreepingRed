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
    
    [SerializeField, Expandable] private List<AreaViewSettingsSO> _uiAreaSettings = new List<AreaViewSettingsSO>();
    public List<AreaViewSettingsSO> AreaUISettings => _uiAreaSettings;
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;

    public override UniTask OnAwake()
    {
        _timeManager = new TimeManager();

        return base.OnAwake();
    }
    
    /// <summary>
    /// エリアのスクリプタブルオブジェクトを登録する
    /// </summary>
    public void RegisterAreas(List<AreaSettingsSO> newAreas)
    {
        _areaSettings.Clear();
        _areaSettings.AddRange(newAreas);
    }
    
    /// <summary>
    /// エリアUIのスクリプタブルオブジェクトを登録する
    /// </summary>
    public void RegisterAreas(List<AreaViewSettingsSO> newAreas)
    {
        _uiAreaSettings.Clear();
        _uiAreaSettings.AddRange(newAreas);
    }
}
