using Cysharp.Threading.Tasks;

/// <summary>
/// エリアをまたぐ感染を管理するクラス
/// </summary>
public class Infection_AcrossAreas
{
    private Grid _grid; // Area情報を取得するためのGridクラスの参照
    
    public Infection_AcrossAreas(Grid grid)
    {
        _grid = grid;
        SpreadEvent().Forget();
    }
    
    /// <summary>
    /// 確率でエリアを跨いだ感染を発生させるクラス
    /// </summary>
    private async UniTask SpreadEvent()
    {
        await UniTask.WaitForSeconds(3);
        
        _grid.Areas[0,3].Spread();
    }
}
