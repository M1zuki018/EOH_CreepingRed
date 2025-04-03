/// <summary>
/// 感染イベント全体を管理するクラス
/// </summary>
public class InfectionEventManager
{
    public InfectionEventManager(Grid grid)
    {
        var acrossAreas = new InfectionAcrossAreas(grid); // イベントを跨ぐ感染を管理するクラス
        var getCost = new GetCost(); // スキル獲得のためのコストを取得するクラス
    }
}
