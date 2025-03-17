using Cysharp.Threading.Tasks;

/// <summary>
/// UITest用のSimulatorクラス
/// </summary>
public class UITestSimulator : ViewBase, ISimulator
{
    private Simulation _simulation;
    private ITimeObservable _timeManager;
    public ITimeObservable TimeManager => _timeManager;

    public override UniTask OnAwake()
    {
        _timeManager = new TimeManager();

        return base.OnAwake();
    }
}
