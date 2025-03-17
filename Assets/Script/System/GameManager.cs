using Cysharp.Threading.Tasks;

/// <summary>
/// GameManager
/// </summary>
public class GameManager : ViewBase
{
    public ITimeObservable TimeManager { get; private set; }
    
    public override UniTask OnAwake()
    {
        TimeManager = FindActiveSimulator().TimeManager;
        
        return base.OnAwake();
    }
    
    /// <summary>
    /// Simulatorクラスを探す
    /// </summary>
    private ISimulator FindActiveSimulator()
    {
        if (FindAnyObjectByType<TestSimulator>() != null)
        {
            return FindAnyObjectByType<TestSimulator>();
        }
        if (FindAnyObjectByType<MiniTestSimulator>() != null)
        {
            return FindAnyObjectByType<MiniTestSimulator>();
        }
        
        return FindAnyObjectByType<UITestSimulator>();
        
    }
}
