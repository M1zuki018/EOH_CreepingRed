/// <summary>
/// 感染イベント全体を管理するクラス
/// </summary>
public class InfectionEventManager
{
    private Infection_AcrossAreas _acrossAreas; // エリアを跨ぐ感染を管理するクラス
    
    public InfectionEventManager(Grid grid)
    {
        _acrossAreas = new Infection_AcrossAreas(grid);
    }
}
