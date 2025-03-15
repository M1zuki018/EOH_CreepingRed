using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// 縮小テスト用Quadtreeを用いたエージェント管理システム
/// </summary>
public class MiniQuadtree
{
    private const int MAX_AGENTS = 2500; // 1区画の最大エージェント数
    private const int MAX_DEPTH = 10; // 最大分割回数
    
    private Dictionary<MiniQuadtree, bool> _subTrees; // サブツリーとSkipフラグ
    private Dictionary<(int x, int y), Agent> _agents; // エージェントの情報
    private Rect _bounds;
    private int _depth = 0; // 現在の分割数
    private int _maxX = 1000; // セル内の座標の横幅の上限
    
    private List<(int, int)> _infectedAgentsCoords = new List<(int, int)>(); // 処理を行う感染済みのエージェント
    private HashSet<(int, int)> _checkAgentCoords = new HashSet<(int, int)>(); // 感染判定を行う対象のエージェント
    private JobHandle _infectionJobHandle;
    private NativeArray<Agent> _agentArray;

    public MiniQuadtree(Rect bounds, int depth = 0)
    {
        _bounds = bounds;
        _depth = depth;
        _subTrees = new Dictionary<MiniQuadtree, bool>();
        _agents = new Dictionary<(int x, int y), Agent>();
        _infectedAgentsCoords = new List<(int, int)>();
        _checkAgentCoords = new HashSet<(int, int)>();
    }

    #region 初期化
    /// <summary>
    /// シミュレーションの初期化処理
    /// </summary>
    public async UniTaskVoid InitializeAgents(int citizen, int magicSoldier)
    {
        var batchSize = 1000; // バッチサイズ
        var tasks = new List<UniTask>();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        // メモリプリアロケーション（必要なエージェント数分のメモリを確保）
        NativeArray<Agent> citizenAgents = new NativeArray<Agent>(citizen, Allocator.TempJob);
        NativeArray<Agent> magicSoldierAgents = new NativeArray<Agent>(magicSoldier, Allocator.TempJob);
        
        // 市民と魔法士のエージェントをバッチ処理で生成
        tasks.AddRange(CreateAgentBatchTasks(citizen, AgentType.Citizen, batchSize, citizenAgents));
        tasks.AddRange(CreateAgentBatchTasks(magicSoldier, AgentType.MagicSoldier, batchSize, magicSoldierAgents));
        
        await UniTask.WhenAll(tasks);  // 全てのタスクが完了するまで待機
        
        // 生成終了後にエージェントをツリーに追加する処理を行う
        InsertBatchAgents(citizenAgents, batchSize);
        InsertBatchAgents(magicSoldierAgents, batchSize);
        
        citizenAgents.Dispose();
        magicSoldierAgents.Dispose();
        
        stopwatch.Stop();
        
        Debug.Log($"一般市民{citizen} 魔法士{magicSoldier} 実行時間: {stopwatch.ElapsedMilliseconds} ミリ秒");
        Debug.Log($"{_subTrees.Count} サブツリー");

        _agents.TryGetValue((0,0), out var test);
        test.State = AgentState.Infected;
        _agents[(0, 0)] = test;
        
        foreach (var agent in _agents)
        {
            Debug.Log($"座標({agent.Key.x}, {agent.Key.y}) 状態{agent.Value.State} スキップ{agent.Value.Skip}");
        }
    }

    /// <summary>
    /// エージェントバッチ生成タスクを作成
    /// </summary>
    private List<UniTask> CreateAgentBatchTasks(int totalCount, AgentType type, int batchSize, NativeArray<Agent> agentArray)
    {
        var tasks = new List<UniTask>();

        for (int i = 0; i < totalCount; i += batchSize)
        {
            int end = Mathf.Min(i + batchSize, totalCount);
            tasks.Add(CreateAgentBatchAsync(i, end, type, agentArray));
        }

        return tasks;
    }

    /// <summary>
    /// バッチ処理用のジョブを実行する
    /// </summary>
    private async UniTask CreateAgentBatchAsync(int start, int end, AgentType type, NativeArray<Agent> agentArray)
    {
        int batchSize = end - start;

        // ジョブを設定
        CreateAgentJob job = new CreateAgentJob
        {
            start = start,
            maxX = _maxX,
            type = type,
            agents = agentArray
        };

        JobHandle jobHandle = job.Schedule(batchSize, 64); // バッチサイズ64で並列化
        jobHandle.Complete(); // 完了まで待機
    }
    
    /// <summary>
    /// エージェントを生成するJob
    /// </summary>
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

    /// <summary>
    /// バッチで生成したエージェントをツリーに追加
    /// </summary>
    private void InsertBatchAgents(NativeArray<Agent> agents, int batchSize)
    {
        int totalAgents = agents.Length;

        for (int i = 0; i < totalAgents; i += batchSize)
        {
            int end = Mathf.Min(i + batchSize, totalAgents);
            InsertBatch(agents, i, end);
        }
    }
    
    /// <summary>
    /// バッチ内でエージェントを一度にツリーに追加
    /// </summary>
    private void InsertBatch(NativeArray<Agent> agents, int start, int end)
    {
        // バッチ内でエージェントを挿入
        for (int i = start; i < end; i++)
        {
            Insert(agents[i]);
        }
    }
    
    /// <summary>
    /// エージェントをツリーに追加する処理
    /// </summary>
    private void Insert(Agent agent)
    {
        if (agent.X < _bounds.xMin || agent.X >= _bounds.xMax || agent.Y < _bounds.yMin || agent.Y >= _bounds.yMax)
        {
            return; // 範囲外なら無視
        }

        if (_agents.Count < MAX_AGENTS || _depth >= MAX_DEPTH)
        {
            _agents[(agent.X, agent.Y)] = agent; // 容量に空きがあり、分割数にも余裕があればエージェントを追加
            return;
        }

        if (_subTrees.Count == 0)
        {
            // 子がいなければ、分割処理を行う
            Subdivide();
        }

        // サブツリーへの追加処理
        foreach (var subTree in _subTrees)
        {
            if (agent.X < subTree.Key._bounds.xMin || agent.X >= subTree.Key._bounds.xMax || 
                agent.Y < subTree.Key._bounds.yMin || agent.Y >= subTree.Key._bounds.yMax)  // サブツリーの範囲内である場合のみ追加
            {
                subTree.Key.Insert(agent);  // このサブツリーにのみエージェントを追加
                break;  // 1つのサブツリーにのみ追加する
            }
        }
    }

    /// <summary>
    /// サブツリーを分割する処理
    /// </summary>
    private void Subdivide() 
    {
        float halfWidth = _bounds.width / 2f;
        float halfHeight = _bounds.height / 2f;
        float x = _bounds.xMin;
        float y = _bounds.yMin;
        
        // 4つのサブツリーを作成
        _subTrees.Add(new MiniQuadtree(new Rect(x, y, halfWidth, halfHeight), _depth + 1), false);  // 左下
        _subTrees.Add(new MiniQuadtree(new Rect(x + halfWidth, y, halfWidth, halfHeight), _depth + 1), false);  // 右下
        _subTrees.Add(new MiniQuadtree(new Rect(x, y + halfHeight, halfWidth, halfHeight), _depth + 1), false);  // 左上
        _subTrees.Add(new MiniQuadtree(new Rect(x + halfWidth, y + halfHeight, halfWidth, halfHeight), _depth + 1), false);  // 右上
    }
    #endregion

    /// <summary>
    /// 感染状態のエージェントで、Skipフラグがfalseのエージェントを探す
    /// </summary>
    public JobHandle SimulateInfection()
    {
        foreach (var agent in _agents)
        {
            if (!agent.Value.Skip && agent.Value.State == AgentState.Infected)
            {
                _infectedAgentsCoords.Add(agent.Key); // 座標をリストに追加
            }
        }

        if (_subTrees.Count > 0)
        {
            foreach (var subTree in _subTrees.Keys)
            {
                _infectionJobHandle = JobHandle.CombineDependencies(
                    _infectionJobHandle, subTree.SimulateInfection()
                );
            }
        }

        return GetInfectedAreas();
    }

    /// <summary>
    /// 感染範囲内のエージェントを収集し、ジョブをスケジュール
    /// </summary>
    private JobHandle GetInfectedAreas()
    {
        foreach (var coord in _infectedAgentsCoords)
        {
            // 対応するエージェントが取得出来なかったら以降の処理を行わないで次
            if (!_agents.TryGetValue(coord, out var infectedAgent))
                continue;
            
            // 感染範囲を定義
            int infectionRange = 2;

            // 周囲の座標を調べ、感染範囲内にいるエージェントを取得
            for (int dx = -infectionRange; dx <= infectionRange; dx++)
            {
                for (int dy = -infectionRange; dy <= infectionRange; dy++)
                {
                    (int, int) neighborCoord = (infectedAgent.X + dx, infectedAgent.Y + dy);

                    // 辞書にその座標のエージェントが存在するか確認
                    if (_agents.ContainsKey(neighborCoord) && !_checkAgentCoords.Contains(neighborCoord))
                    {
                        // 自身以外のエージェントであれば感染判定
                        _checkAgentCoords.Add(neighborCoord);
                    }
                }
            }
        }
        
        if (_subTrees.Count > 0)
        {
            foreach (var subTree in _subTrees.Keys)
            {
                _infectionJobHandle = JobHandle.CombineDependencies(
                    _infectionJobHandle, subTree.GetInfectedAreas()
                );
            }
        }

        return InfectionDetermination();
    }

    /// <summary>
    /// 感染判定を並列処理で一斉に行う
    /// </summary>
    private JobHandle InfectionDetermination()
    {
        _agentArray = new NativeArray<Agent>(_checkAgentCoords.Count, Allocator.TempJob);
        
        // エージェントを辞書からNativeArrayに変換
        int index = 0;
        foreach (var coord in _checkAgentCoords)
        {
            if (_agents.TryGetValue(coord, out var agent))
            {
                _agentArray[index] = agent;
                index++;
            }
        }
        
        InfectionJob job = new InfectionJob
        {
            agents = _agentArray,
            baseInfectionRate = 5f,
            infectionMultiplier = 2
        };
        
        JobHandle jobHandle = job.Schedule(_agentArray.Length, 64); // 64スレッド単位で並列処理
        
        // ジョブが完了した後に Dispose を呼び出す
        jobHandle.Complete();
        
        // 結果を _agents に反映
        for (int i = 0; i < _agentArray.Length; i++)
        {
            var updatedAgent = _agentArray[i];
            var coord = (updatedAgent.X, updatedAgent.Y);

            if (_agents.ContainsKey(coord))
            {
                _agents[coord] = updatedAgent; // 更新を反映
            }
        }

        foreach (var agent in _agents)
        {
            Debug.Log($"座標({agent.Key.x}, {agent.Key.y}) 状態{agent.Value.State} スキップ{agent.Value.Skip}");
        }
        
        // ジョブ処理後に _agentArray を Dispose する
        _agentArray.Dispose();
        
        return jobHandle;
    }
    
    public IEnumerable<Agent> GetAllAgents()
    {
        foreach (var agent in _agents.Values)
        {
            yield return agent;
        }

        foreach (var subTree in _subTrees.Keys)
        {
            foreach (var agent in subTree.GetAllAgents())
            {
                yield return agent;
            }
        }
    }

    [BurstCompile]
    private struct InfectionJob : IJobParallelFor
    {
        public NativeArray<Agent> agents;
        public float baseInfectionRate;
        public float infectionMultiplier;

        public void Execute(int index)
        {
            Agent agent = agents[index];
            
            float infectionProbability = baseInfectionRate * infectionMultiplier + 20; // 感染する確率

            // 感染判定
            if (infectionProbability > 10)
            {
                agent.State = AgentState.Infected;
            }
            
            agents[index] = agent; // 変更を反映
        }
    }
}
