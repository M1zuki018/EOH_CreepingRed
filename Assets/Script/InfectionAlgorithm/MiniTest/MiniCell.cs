using Cysharp.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// ※3/22 魔法士は感染シミュレーションには入れずに、イベント側で扱う
/// </summary>
public class MiniCell
{
    private int _id; // セル自体のID
    
    private MiniQuadtree _quadtree;
    private JobHandle _quadtreeJobHandle;
    public AgentStateCount StateCount { get; private set; }
    
    public MiniCell(int id, int citizen, float regionMod)
    {
        _id = id;
        _quadtree = new MiniQuadtree(new Rect(0, 0, 1000, 1000), regionMod);
        StateCount = new AgentStateCount();
        
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
            // Quadtree のシミュレーション開始
            _quadtreeJobHandle = _quadtree.SimulateInfection();

            // すべての `Quadtree` のジョブが完了するまで待機
            _quadtreeJobHandle.Complete();
        },$"\ud83d\udfe6セル(ID:{_id}) 更新速度");
        // ここでエージェントの状態をカウント
        UpdateStateCount();

        await UniTask.CompletedTask;
    }
    
    /// <summary>
    /// 各状態のエージェントの数を集計する
    /// </summary>
    private void UpdateStateCount()
    {
        StateCount.ResetStateCount();
        foreach (var agent in _quadtree.GetAllAgents())
        {
            StateCount.AddState(agent.State); // 各ステート
        }
    }
}
