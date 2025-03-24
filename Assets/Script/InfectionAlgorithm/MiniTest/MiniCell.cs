using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class MiniCell
{
    private readonly int _id; // セル自体のID
    private readonly Dictionary<MiniAgentManager, (bool skip, bool infection)> _agentManager = new Dictionary<MiniAgentManager, (bool, bool)>(); // AgentManagerにリスト
    private readonly AgentStateCount _cellStateCount;
    public AgentStateCount CellStateCount => _cellStateCount; // エージェントのカウント用のクラス

    private const int ManagerCapacity = 10000; // 一つのAgentManagerクラスに持たせるAgentの総数の上限
    private readonly List<JobHandle> _jobHandle = new List<JobHandle>(); // エージェント生成JobのHandle
    
    public MiniCell(int id, int citizen, float regionMod)
    {
        _id = id;
        _cellStateCount = new AgentStateCount();
        
        int managerCount = Mathf.CeilToInt((float)citizen / ManagerCapacity); // 人口÷エージェント総数の上限
        
        for(int i = 0; i < managerCount; i++)
        {
            _agentManager[new MiniAgentManager(regionMod)] = (false, false); // 必要な個数分MiniAgentManagerを作成
        }
        
        StopwatchHelper.Measure(() => InitializeAgents(citizen).Forget(),"Quadtree生成完了");
    }

    /// <summary>
    /// エージェントの生成（非同期でセットアップ）
    /// </summary>
    private async UniTask InitializeAgents(int citizen)
    {
        List<UniTask> tasks = new List<UniTask>(_agentManager.Count);
        
        bool hasInitialized = false;
        
        foreach (var kvp in _agentManager)
        {
            tasks.Add(kvp.Key.InitializeAgents(citizen, !hasInitialized));
            hasInitialized = true;
        }
        
        await UniTask.WhenAll(tasks); // 全ての生成が終わるまで待つ
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public async UniTask SimulateInfection()
    {
        _jobHandle.Clear();
        
        StopwatchHelper.Measure(() =>
        {
            foreach (var kvp in _agentManager)
            {
                if (kvp.Value.skip) // スキップでなければ
                {
                    _jobHandle.Add(kvp.Key.SimulateInfection()); 
                }
            }

            if (_jobHandle.Count > 0)
            {
                using var jobHandles = new NativeArray<JobHandle>(_jobHandle.ToArray(), Allocator.TempJob);
                JobHandle.CompleteAll(jobHandles); // すべてのQuadtreeのジョブが完了するまで待機
            }

            
                
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
            if (_agentManager.Count == 0) return;
            
            HashSet<MiniAgentManager> skipManagers = new HashSet<MiniAgentManager>();
            HashSet<MiniAgentManager> restartManagers = new HashSet<MiniAgentManager>();
            HashSet<MiniAgentManager> infectionManagers = new HashSet<MiniAgentManager>();
            
            
            foreach (var kvp in _agentManager)
            {
                var allAgents = kvp.Key.GetAllAgents(); // AgentManagerから全てのエージェントを取得する
            
                bool isAllHealthy = true; // 全てのエージェントが感染しているか
                int infectedCount = 0; // 感染者の人数
                int totalAgents = allAgents.Count();
                
                foreach (var agent in allAgents)
                {
                    _cellStateCount.AddState(agent.State); // 各ステートをカウント
                    if (agent.State == AgentState.Infected)
                    {
                        isAllHealthy = false;
                        infectedCount++;
                    }
                }
                
                float infectionRatio = (float)infectedCount / totalAgents;
                
                // 感染者が６割以上なら感染処理を行う
                if (infectionRatio >= 0.6f && !kvp.Value.infection)
                {
                    infectionManagers.Add(kvp.Key);
                }
                

                if (!isAllHealthy || infectedCount != totalAgents)
                {
                    if(!kvp.Value.skip) restartManagers.Add(kvp.Key);
                    continue;
                }
                
                // 全員が健康状態 もしくは 全員が感染状態の場合、スキップするようにする
                skipManagers.Add(kvp.Key);
            }
            
            // 再起動処理
            foreach (var restart in restartManagers)
            {
                _agentManager[restart] = (true, _agentManager[restart].infection);
            }
            
            // スキップ処理
            foreach (var skip in skipManagers)
            {
                _agentManager[skip] = (false, _agentManager[skip].infection);
            }

            if (infectionManagers.Count > 0)
            {
                for (int i = 0; i < infectionManagers.Count; i++)
                {
                    
                }
            }
            // 感染拡大処理
            foreach (var infection in infectionManagers)
            {
                
            }
            
        }, $"\ud83d\udfe6セル(ID:{_id}) Quadtreeのステート集計速度");
    }
}
