using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// GameManager
/// </summary>
public class GameManager : ViewBase
{
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;
    
    public override UniTask OnAwake()
    {
        _timeManager = new TimeManager();
        var simulator = FindActiveSimulator();
        if (simulator != null)
        {
            simulator.Initialize(_timeManager);
        }
        else
        {
            Debug.LogError("\u274c\u274c\u274c GameManager : Simulatorクラスの初期化が完了していません \u274c\u274c\u274c");
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
