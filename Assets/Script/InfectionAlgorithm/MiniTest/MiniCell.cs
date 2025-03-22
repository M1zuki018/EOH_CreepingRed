using Cysharp.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// ※3/22 魔法士は感染シミュレーションには入れずに、イベント側で扱う
/// </summary>
public class MiniCell
{
    private readonly int _id; // セル自体のID
    
    private MiniQuadtree _quadtree;
    private AgentStateCount _cellStateCount;
    public AgentStateCount CellStateCount => _cellStateCount;
    
    private JobHandle _quadtreeJobHandle;
    
    public MiniCell(int id, int citizen, float regionMod)
    {
        _id = id;
        
        // 感染確率を渡してQuadtreeを作成。深さは初期値のゼロ
        _quadtree = new MiniQuadtree(new Rect(0, 0, 1000, 1000), regionMod);
        _cellStateCount = new AgentStateCount();
        
        InitializeAgents(citizen).Forget();
    }

    /// <summary>
    /// エージェントの生成（非同期でセットアップ）
    /// </summary>
    private async UniTask InitializeAgents(int citizen)
    {
        await _quadtree.InitializeAgents(citizen);
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public async UniTask SimulateInfection()
    {
        StopwatchHelper.Measure(() =>
        {
            _quadtreeJobHandle = _quadtree.SimulateInfection(); // Quadtreeの更新処理
            _quadtreeJobHandle.Complete(); // すべてのQuadtreeのジョブが完了するまで待機
        },$"\ud83d\udfe6セル(ID:{_id}) 感染シミュレーションの更新速度");
        
        await UpdateStateCount();
    }
    
    /// <summary>
    /// Quadtree内のエージェントのステートを集計する
    /// </summary>
    private async UniTask UpdateStateCount()
    {
        _cellStateCount.ResetStateCount();
        
        var allAgents = _quadtree.GetAllAgents();
        
        await StopwatchHelper.MeasureAsync(async () =>
        {
            foreach (var agent in allAgents)
            {
                _cellStateCount.AddState(agent.State); // 各ステートをカウント
            }
        }, $"\ud83d\udfe6セル(ID:{_id}) Quadtreeのステート集計速度");
    }
}
