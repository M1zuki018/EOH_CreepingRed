using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// GameManager
/// </summary>
public class GameManager : ViewBase
{
    public ITimeObservable TimeManager { get; private set; }
    public List<AreaSettingsSO> AreaSettings { get; private set; }
    public List<AreaViewSettingsSO> AreaUISettings { get; private set; }
    
    public override UniTask OnAwake()
    {
        ISimulator simulator = FindActiveSimulator();
        AreaSettings = simulator.AreaSettings;
        AreaUISettings = simulator.AreaUISettings;
        TimeManager = simulator.TimeManager;
        if (TimeManager == null)
        {
            // Simulatorクラスを生成したあとに呼ばないとnullになるので注意
            Debug.LogError("\u274c\u274c\u274c GameManager : TimeManagerが取得できませんでした \u274c\u274c\u274c" +
                           "\ud83d\udea8 実行順がSimulator -> GameManagerであることを確認してください\ud83d\udea8");
        }
        
        return base.OnAwake();
    }
    
    /// <summary>
    /// Simulatorクラスを探す
    /// </summary>
    private ISimulator FindActiveSimulator()
    {
        return (ISimulator)FindAnyObjectByType<TestSimulator>() ??
               (ISimulator)FindAnyObjectByType<MiniTestSimulator>() ??
               (ISimulator)FindAnyObjectByType<UITestSimulator>();
    }
}
