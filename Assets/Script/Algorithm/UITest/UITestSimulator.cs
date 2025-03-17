using Cysharp.Threading.Tasks;

/// <summary>
/// 
/// </summary>
public class UITestSimulator : ViewBase, ISimulator
{
    private Simulation _simulation;
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;

    public override UniTask OnStart()
    {
        _timeManager = new TimeManager();

        return base.OnStart();
    }
}
