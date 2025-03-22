using System.Threading;
using System.Threading.Tasks;
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
    private JobHandle _quadtreeJobHandle;
    private AgentStateCount _cellStateCount;
    public AgentStateCount CellStateCount => _cellStateCount;
    
    public MiniCell(int id, int citizen, float regionMod)
    {
        _id = id;
        
        // 感染確率を渡してQuadtreeを作成。深さは初期値のゼロ
        _quadtree = new MiniQuadtree(new Rect(0, 0, 1000, 1000), regionMod);
        _cellStateCount = new AgentStateCount();
        
        InitializeAgents(citizen);
    }

    /// <summary>
    /// エージェントの生成
    /// </summary>
    private void InitializeAgents(int citizen)
    {
        _quadtree.InitializeAgents(citizen).Forget();
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public async UniTask SimulateInfection()
    {
        StopwatchHelper.Measure(() =>
        {
            _quadtreeJobHandle = _quadtree.SimulateInfection(); // Quadtreeの更新処理
            _quadtreeJobHandle.Complete(); // すべての `Quadtree` のジョブが完了するまで待機
        },$"\ud83d\udfe6セル(ID:{_id}) 感染シミュレーションの更新速度");
        
        await UpdateStateCount();
    }
    
    /// <summary>
    /// Quadtree内のエージェントのステートを集計する
    /// </summary>
    private async UniTask UpdateStateCount()
    {
        _cellStateCount.ResetStateCount();
        
        StopwatchHelper.Measure(() =>
        {
            foreach (var agent in _quadtree.GetAllAgents())
            {
                _cellStateCount.AddState(agent.State); // 各ステート
            }
        }, $"\ud83d\udfe6セル(ID:{_id}) Quadtreeのステート集計速度");
        
        await UniTask.CompletedTask;
    }
}
