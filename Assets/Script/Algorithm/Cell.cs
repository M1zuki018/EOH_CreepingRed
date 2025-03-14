using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class Cell
{
    private int _id; // セル自体のID
    private int _maxX = 1000; // セル内の座標の横幅の上限
    private List<Agent> _agents { get; } = new List<Agent>();
    private Quadtree _quadtree;
    public AgentStateCount StateCount { get; private set; }
    
    public Cell(int id, int citizen, int magicSoldier)
    {
        _id = id;
        _agents = new List<Agent>(citizen + magicSoldier);
        StateCount = new AgentStateCount();
        
        InitializeAgents(citizen, magicSoldier).Forget();
    }

    #region 初期化

    /// <summary>
    /// エージェントの生成
    /// </summary>
    private async UniTaskVoid InitializeAgents(int citizen, int magicSoldier)
    {
        var batchSize = 1000; // バッチサイズ
        var tasks = new List<UniTask>();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        // 市民と魔法士のエージェントをバッチ処理で生成
        tasks.AddRange(CreateAgentBatchTasks(citizen, AgentType.Citizen, batchSize));
        tasks.AddRange(CreateAgentBatchTasks(magicSoldier, AgentType.MagicSoldier, batchSize));
        
        await UniTask.WhenAll(tasks);  // 全てのタスクが完了するまで待機
        
        tasks.Add(UniTask.RunOnThreadPool(() => UpdateStateCount(magicSoldier)));
        
        await UniTask.WhenAll(tasks);
        
        stopwatch.Stop();
        
        Debug.Log($"{_id} : 一般市民{citizen} 魔法士{magicSoldier} 実行時間: {stopwatch.ElapsedMilliseconds} ミリ秒");
    }

    private void UpdateStateCount(int magicSoldier)
    {
        StateCount.ResetStateCount();
        foreach (var agent in _agents)
        {
            StateCount.AddState(agent.State); // 各ステート
            StateCount.AddState(magicSoldier); // 魔法士の数
        }
    }

    /// <summary>
    /// エージェントバッチ生成タスクを作成
    /// </summary>
    private List<UniTask> CreateAgentBatchTasks(int totalCount, AgentType type, int batchSize)
    {
        var tasks = new List<UniTask>();

        for (int i = 0; i < totalCount; i += batchSize)
        {
            int end = Mathf.Min(i + batchSize, totalCount);
            tasks.Add(CreateAgentBatchAsync(i, end, type));
        }

        return tasks;
    }
    
    /// <summary>
    /// バッチ処理用のジョブを実行する
    /// </summary>
    private async UniTask CreateAgentBatchAsync(int start, int end, AgentType type)
    {
        int batchSize = end - start;
        NativeArray<Agent> agentArray = new NativeArray<Agent>(batchSize, Allocator.TempJob);

        // ジョブを設定
        CreateAgentJob job = new CreateAgentJob
        {
            start = start,
            maxX = _maxX,
            type = type,
            agents = agentArray
        };

        JobHandle jobHandle = job.Schedule(batchSize, 64);  // バッチサイズ64で並列化
        jobHandle.Complete();  // 完了まで待機

        // NativeArrayをリストに変換してエージェントに追加
        for (int i = 0; i < batchSize; i++)
        {
            _agents.Add(agentArray[i]);
        }

        // メモリ解放
        agentArray.Dispose();
    }
    
    [BurstCompile]
    private struct CreateAgentJob : IJobParallelFor
    {
        public int start;
        public int maxX;
        public AgentType type;
        public NativeArray<Agent> agents;

        public void Execute(int index)
        {
            // 座標の計算
            int agentIndex = start + index;
            int x = agentIndex % maxX;
            int y = agentIndex / maxX;

            // エージェントを追加
            agents[index] = new Agent(agentIndex, type, x, y);
        }
    }

    #endregion

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public void SimulateInfection(float baseInfectionRate, float infectionMultiplier)
    {
        if(_agents.Count == 0) return; // エージェントがセル内に一人もいない場合スキップ
        
        var infectedAreas = _quadtree.GetInfectedAreas();
        if(infectedAreas.Count == 0) return; // 感染者がいない場合スキップ
        
        NativeArray<Agent> agentArray = new NativeArray<Agent>(infectedAreas.Count, Allocator.TempJob);
        NativeArray<bool> infectionResults = new NativeArray<bool>(infectedAreas.Count, Allocator.TempJob);
        
        // リストから NativeArray へコピー
        for (int i = 0; i < infectedAreas.Count; i++)
        {
            agentArray[i] = infectedAreas[i];
        }

        // Job作成
        InfectionJob job = new InfectionJob
        {
            agents = agentArray,
            infectionResults = infectionResults,
            baseInfectionRate = baseInfectionRate,
            infectionMultiplier = infectionMultiplier,
        };
        
        JobHandle jobHandle = job.Schedule(infectedAreas.Count, 64); // 64スレッド単位で並列処理
        jobHandle.Complete(); // 終了を待つ
        
        // 感染結果を反映
        for (int i = 0; i < infectedAreas.Count; i++)
        {
            if (infectionResults[i])
            {
                _agents[i].Infect();
            }
        }

        // メモリ解放
        agentArray.Dispose();
        infectionResults.Dispose();
    }
    
    [BurstCompile]
    private struct InfectionJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Agent> agents;
        public NativeArray<bool> infectionResults;
        public float baseInfectionRate;
        public float infectionMultiplier;

        public void Execute(int index)
        {
            Agent agent = agents[index];
            if (agent.State == AgentState.Infected)
            {
                infectionResults[index] = false;
                return;
            }

            int infectedNeighbors = agent.CountInfectedNeighbors(); // 感染した近隣の人間の数を取得
            float infectionProbability = baseInfectionRate + (infectedNeighbors * infectionMultiplier);

            // 感染判定
            infectionResults[index] = 10 < infectionProbability;
        }
    }
}
