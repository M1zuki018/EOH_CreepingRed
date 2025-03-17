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
