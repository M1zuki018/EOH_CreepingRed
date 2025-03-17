using System.Collections.Generic;

/// <summary>
/// Simulatorクラスのインターフェース
/// </summary>
public interface ISimulator
{
    ITimeObservable TimeManager { get; }
    List<AreaSettingsSO> AreaSettings { get; }
}
