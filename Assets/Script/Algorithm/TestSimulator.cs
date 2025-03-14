using Cysharp.Threading.Tasks;

/// <summary>
/// シミュレーションテスト用のクラス
/// </summary>
public class TestSimulator : ViewBase
{
    Simulation _simulation;
    
    public override UniTask OnStart()
    {
        // ヨコ5マス×タテ4マスのグリッド
        // 人口は9,130万人
        _simulation = new Simulation(5, 4, 91300000);
        
        return base.OnStart();
    }

    public void Update()
    {
        //_simulation.Run();
    }
}
