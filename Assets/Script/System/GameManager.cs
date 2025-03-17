using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// GameManager
/// </summary>
public class GameManager : ViewBase
{
    public ITimeObservable TimeManager { get; private set; }
    
    public override UniTask OnAwake()
    {
        TimeManager = FindActiveSimulator().TimeManager;
        if (TimeManager == null)
        {
            Debug.LogError("\u274c\u274c\u274c GameManager : TimeManagerが取得できませんでした \u274c\u274c\u274c");
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
