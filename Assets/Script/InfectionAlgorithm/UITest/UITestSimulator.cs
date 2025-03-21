using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UITest用のSimulatorクラス
/// </summary>
public class UITestSimulator : ViewBase, ISimulator
{
    [SerializeField, HighlightIfNull] private List<AreaSettingsSO> _areaSettings;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(ITimeObservable timeManager) { } // 現状処理なし
    
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
