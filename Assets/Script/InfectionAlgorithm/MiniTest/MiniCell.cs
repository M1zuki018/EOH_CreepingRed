using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// ※3/22 魔法士は感染シミュレーションには入れずに、イベント側で扱う
/// </summary>
public class MiniCell
{
    private readonly int _id; // セル自体のID
    
    private readonly MiniQuadtree _quadtree;
    private readonly Dictionary<MiniAgentManager, bool> _agentManager = new Dictionary<MiniAgentManager, bool>();
    private readonly AgentStateCount _cellStateCount;
    public AgentStateCount CellStateCount => _cellStateCount;

    private int _managerCapacity = 10000;
    
    private List<JobHandle> _quadtreeJobHandle = new List<JobHandle>();
    
    public MiniCell(int id, int citizen, float regionMod)
    {
        _id = id;
        
        // 感染確率を渡してQuadtreeを作成。深さは初期値のゼロ
        //_quadtree = new MiniQuadtree(new Rect(0, 0, 1000, 1000), regionMod);
        for(int i = 0; i < Mathf.Ceil(citizen / _managerCapacity); i++)
        {
            _agentManager.Add(new MiniAgentManager(regionMod), false);
        }
        _cellStateCount = new AgentStateCount();

        StopwatchHelper.Measure(() =>
        {
            InitializeAgents(citizen).Forget();
        },"Quadtree生成完了");
    }

    /// <summary>
    /// エージェントの生成（非同期でセットアップ）
    /// </summary>
    private async UniTask InitializeAgents(int citizen)
    {
        List<UniTask> tasks = new List<UniTask>(_agentManager.Count);
        
        foreach (var agentManager in _agentManager.Keys)
        {
            tasks.Add(agentManager.InitializeAgents(citizen));
        }
        
        await UniTask.WhenAll(tasks);
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public async UniTask SimulateInfection()
    {
        _quadtreeJobHandle.Clear();
        
        StopwatchHelper.Measure(() =>
        {
            foreach (var agentManager in _agentManager.Keys)
            {
                if (_agentManager[agentManager])
                {
                    _quadtreeJobHandle.Add(agentManager.SimulateInfection()); // Quadtreeの更新処理
                }
            }
            
            //_quadtreeJobHandle.Complete(); // すべてのQuadtreeのジョブが完了するまで待機
        },$"\ud83d\udfe6セル(ID:{_id}) 感染シミュレーションの更新速度");
        
        await UpdateStateCount();
    }
    
    /// <summary>
    /// Quadtree内のエージェントのステートを集計する
    /// </summary>
    private async UniTask UpdateStateCount()
    {
        _cellStateCount.ResetStateCount();
        
        await StopwatchHelper.MeasureAsync(async () =>
        {
            foreach (var agentManager in _agentManager.Keys)
            {
                var allAgents = agentManager.GetAllAgents();
            
                bool isAllHealthy = true;
                int infectedCount = 0;
                
                foreach (var agent in allAgents)
                {
                    _cellStateCount.AddState(agent.State); // 各ステートをカウント
                    if (agent.State == AgentState.Infected)
                    {
                        isAllHealthy = false;
                        infectedCount++;
                    }
                }

                if (!isAllHealthy)
                {
                    _agentManager[agentManager] = false;
                }
            }
            
        }, $"\ud83d\udfe6セル(ID:{_id}) Quadtreeのステート集計速度");
    }
}
