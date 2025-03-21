using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 縮小テスト用Quadtreeを用いたエージェント管理システム
/// </summary>
public class MiniQuadtree
{
    // 定数
    private const int MAX_AGENTS = 5000; // 1ツリーの最大エージェント数
    private const int MAX_DEPTH = 10; // 最大分割回数
    private const int MERGE_THRESHOLD = 4000; // 統合の閾値
    
    // 初期化処理
    private List<UniTask> _generateTasks; // エージェント生成タスクのリスト
    
    // QuadTree関連
    private Dictionary<MiniQuadtree, bool> _subTrees; // サブツリーとSkipフラグ
    private Dictionary<(int x, int y), Agent> _agents; // エージェントの情報
    private Rect _bounds;
    private readonly int _depth = 0; // 現在の分割数
    private readonly int _maxX = 1000; // セル内の座標の横幅の上限
    
    // シミュレーション関連
    private List<(int, int)> _infectedAgentsCoords; // 処理を行う感染済みのエージェント
    private HashSet<(int, int)> _checkAgentCoords; // 感染判定を行う対象のエージェント
    private List<(int, int)> _nearDeathAgentsCoords = new List<(int, int)>(); // 仮死判定を行う対象のエージェント
    private JobHandle _infectionJobHandle;
    private NativeArray<Agent> _agentArray; // 感染判定を行うエージェントのNativeArray
    private NativeArray<Agent> _nearDeathAgentsArray; // 死亡判定を行うエージェントのNativeArray
    private readonly object _lockObject = new object();
    private readonly float _regionMod; // 感染確率計算の環境補正
    private float _difficultyMod; // 感染確率計算の難易度補正
    private int _infectionRange; // 感染範囲

    public MiniQuadtree(Rect bounds, float regionMod, int depth = 0)
    {
        _bounds = bounds;
        _regionMod = regionMod;
        _depth = depth;
        _subTrees = new Dictionary<MiniQuadtree, bool>();
        _agents = new Dictionary<(int x, int y), Agent>();
        _infectedAgentsCoords = new List<(int, int)>();
        _checkAgentCoords = new HashSet<(int, int)>();
        _difficultyMod = GameSettingsManager.Difficulty switch
        {
            DifficultyEnum.Breeze => 0,
            DifficultyEnum.Storm => 0.05f, // 普通
            DifficultyEnum.Catastrophe => 0.1f, //難しい
            DifficultyEnum.Unknown => 0.2f, // 激ムズ
            DifficultyEnum.Custom => 0f, // カスタム難易度
            _ => 0,
        };
    }

    #region 初期化
    /// <summary>
    /// シミュレーションの初期化処理
    /// ※大元のQuadtreeでのみ呼ばれる
    /// </summary>
    public async UniTask InitializeAgents(int citizen)
    {
        await GenerateAgents(citizen);
        
        TestInfection();
        
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 一人だけ感染させる処理
    /// ※大元のQuadtreeでのみ呼ばれる
    /// </summary>
    private void TestInfection()
    {
        MiniQuadtree targetTree = FindTargetTree(new Agent (99999, AgentType.Citizen, 0, 0)); // (0,0)座標のツリーを検索

        if (targetTree._agents.TryGetValue((0, 0), out var test))
        {
            test.Infect();
            targetTree._agents[(0, 0)] = test; // 更新
            Debug.Log($"Agent at (0,0) infected in tree with bounds: {targetTree._bounds}");
        }
        else
        {
            Debug.LogWarning("No agent found at (0,0) in the target Quadtree.");
        }
    }

    /// <summary>
    /// エージェントを並列処理で生成する
    /// ※大元のQuadtreeでのみ呼ばれる
    /// </summary>
    private async UniTask GenerateAgents(int citizen)
    {
        NativeArray<Agent> agentArray = new NativeArray<Agent>(citizen, Allocator.TempJob);
        
        var job = new AgentInitializationJob
        {
            Agents = agentArray,
            MaxX = _maxX
        };
        
        JobHandle handle = job.Schedule(citizen, 64);
        handle.Complete();
        
        // サブツリーを作成
        int requiredDepth = CalculateRequiredDepth(citizen);
        EnsureSubTrees(requiredDepth);
        
        // 直接適切な辞書にエージェントを追加
        for (int i = 0; i < citizen; i++)
        {
            InsertDirectly(agentArray[i]);
        }
        
        agentArray.Dispose();
        
        await UniTask.CompletedTask; // 全てのエージェントの生成を待つ
    }
    
    /// <summary>
    /// 必要なQuadtreeの分割深度を計算
    /// ※大元のQuadtreeでのみ呼ばれる
    /// </summary>
    private int CalculateRequiredDepth(int citizen)
    {
        int depth = 0;
        int capacity = MAX_AGENTS;

        while (citizen > capacity)
        {
            capacity *= 4; // クワッドツリーは1分割ごとに4倍
            depth++;
        }

        return depth;
    }
    
    /// <summary>
    /// 必要なサブツリーを確保する処理
    /// </summary>
    private void EnsureSubTrees(int requiredDepth)
    {
        if (_depth >= requiredDepth) return; // すでに十分な深さなら何もしない

        if (_depth < requiredDepth) // 現在のツリーが十分な深さに達していない場合分割を行う
        {
            Subdivide(); // 分割を実行
        }

        foreach (var subtree in _subTrees)
        {
            subtree.Key.EnsureSubTrees(requiredDepth); // サブツリーが生成されていたら再帰的に処理を行う
        }
    }
    
    /// <summary>
    /// エージェントを最適なツリーの辞書に直接追加
    /// ※大元のQuadtreeでのみ呼ばれる
    /// </summary>
    private void InsertDirectly(Agent agent)
    {
        MiniQuadtree targetTree = FindTargetTree(agent);
        targetTree._agents[(agent.X, agent.Y)] = agent;
    }
    
    /// <summary>
    /// エージェントを適切なサブツリーに割り当てる
    /// ※大元のQuadtreeでのみ呼ばれる
    /// </summary>
    private MiniQuadtree FindTargetTree(Agent agent)
    {
        MiniQuadtree currentTree = this;
        Vector2 agentPos = new Vector2(agent.X, agent.Y);
    
        while (currentTree._subTrees.Count > 0)
        {
            MiniQuadtree bestTree = null;
            foreach (var subTree in currentTree._subTrees.Keys)
            {
                if (subTree._bounds.Contains(agentPos))
                {
                    bestTree = subTree;
                    break; // 最初に見つかったものを採用
                }
            }

            if (bestTree == null)
            {
                Debug.LogWarning($"Agent ({agent.X}, {agent.Y}) が適切なサブツリーに属していません。現在のツリー: {currentTree._bounds}");
                break;
            }

            currentTree = bestTree;
        }

        return currentTree;
    }
    
    [BurstCompile]
    struct AgentInitializationJob : IJobParallelFor
    {
        [WriteOnly] public NativeArray<Agent> Agents;
        public int MaxX;

        public void Execute(int i)
        {
            int x = i % MaxX;
            int y = i / MaxX;

            Agents[i] = new Agent(i, AgentType.Citizen, x, y);
        }
    }

    #endregion
    
    #region ツリーを操作する処理
    
    /// <summary>
    /// エージェントをツリーに追加する処理
    /// </summary>
    private void Insert(Agent agent)
    {
        // 範囲外なら無視
        if (agent.X < _bounds.xMin || agent.X >= _bounds.xMax || agent.Y < _bounds.yMin || agent.Y >= _bounds.yMax)
            return; 
        
        // 容量に空きがあったらそのままエージェントを追加
        if (_agents.Count < MAX_AGENTS)
        {
            _agents[(agent.X, agent.Y)] = agent; 
            return;
        }

        // 分割数に余裕がない場合そのままエージェントを追加
        if (_depth >= MAX_DEPTH) 
        {
            _agents[(agent.X, agent.Y)] = agent;
            return;
        }
        
        // 容量に空きがなく、分割数に余裕があるのでサブツリーに分割
        AddAgentToSubTree(agent);
    }

    /// <summary>
    ///  エージェントをサブツリーに追加する
    /// </summary>
    private void AddAgentToSubTree(Agent agent)
    {
        if (_subTrees.Count == 0)
        {
            Subdivide();
        }
        
        // エージェントの位置に基づいてサブツリーに振り分け
        foreach (var subTree in _subTrees.Keys.ToList())
        {
            // エージェントがサブツリーの範囲内に収まっているかをチェック
            if (subTree._bounds.Contains(new Vector2(agent.X, agent.Y)))
            {
                // サブツリーにエージェントを追加
                subTree.Insert(agent);
                return;
            }
        }

        // もし適切なサブツリーが見つからなければ、通常のエージェント追加処理
        _agents[(agent.X, agent.Y)] = agent;
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
        var leftBottom  = new MiniQuadtree(new Rect(x, y, halfWidth, halfHeight), _regionMod, _depth + 1);
        var rightBottom = new MiniQuadtree(new Rect(x + halfWidth, y, halfWidth, halfHeight), _regionMod, _depth + 1);
        var leftTop     = new MiniQuadtree(new Rect(x, y + halfHeight, halfWidth, halfHeight), _regionMod, _depth + 1);
        var rightTop    = new MiniQuadtree(new Rect(x + halfWidth, y + halfHeight, halfWidth, halfHeight), _regionMod, _depth + 1);

        _subTrees.Add(leftBottom, false);
        _subTrees.Add(rightBottom, false);
        _subTrees.Add(leftTop, false);
        _subTrees.Add(rightTop, false);

        // デバッグ用ログ
        Debug.Log($"サブツリー生成　深さ{_depth}: \n" +
                  $"Left Bottom: {leftBottom._bounds} \n" +
                  $"Right Bottom: {rightBottom._bounds} \n" +
                  $"Left Top: {leftTop._bounds} \n" +
                  $"Right Top: {rightTop._bounds}");
    }

    /// <summary>
    /// サブツリーを統合する処理（一定以下のエージェント数なら統合する）
    /// </summary>
    private void Marge()
    {
        if (_subTrees.Count == 0)
            return; // すでにリーフノードなら何もしない

        // 子ノード内のエージェント数を合計
        int totalAgents = 0;
        foreach (var subTree in _subTrees.Keys)
        {
            totalAgents += subTree._agents.Count;
        }

        // 子ノードの合計エージェント数が閾値以下なら統合
        if (totalAgents <= MERGE_THRESHOLD)
        {
            // すべてのエージェントを現在のノードに統合
            foreach (var subTree in _subTrees.Keys)
            {
                foreach (var agent in subTree._agents.Values)
                {
                    _agents[(agent.X, agent.Y)] = agent;
                }
            }

            // サブツリーを削除
            _subTrees.Clear();
            Debug.Log("サブツリーを削除");
        }
    }
    
    #endregion

    #region 感染シミュレーション

    /// <summary>
    /// 感染状態のエージェントで、Skipフラグがfalseのエージェントを探す
    /// </summary>
    public JobHandle SimulateInfection()
    {
        // リストを最初にクリアしておく
        _infectedAgentsCoords.Clear();
        _checkAgentCoords.Clear();
        _infectionRange = InfectionParameters.InfectionRange; // 感染範囲を更新
        
        foreach (var agent in _agents)
        {
            if (agent.Value.State == AgentState.Infected)
            {
                if (!agent.Value.Skip)
                {
                    _infectedAgentsCoords.Add(agent.Key); // 周囲を感染させる可能性があるエージェントをリストに追加
                }
                _nearDeathAgentsCoords.Add(agent.Key); // 死亡判定を行うリストに追加
            }
        }

        if (_subTrees.Count > 0)
        {
            foreach (var subTree in _subTrees.Keys)
            {
                // サブツリーの感染処理も待つようにする
                _infectionJobHandle = JobHandle.CombineDependencies(_infectionJobHandle, subTree.SimulateInfection());
            }
        }

        NearDeathDetermination(); // 死亡判定
        return GetInfectedAreas();
    }

    /// <summary>
    /// 感染範囲内のエージェントを収集し、ジョブをスケジュール
    /// </summary>
    private JobHandle GetInfectedAreas()
    {
        HashSet<(int, int)> coordsToCheck = new HashSet<(int, int)>();
        HashSet<(int, int)> coordsToMarkSkip = new HashSet<(int, int)>();

        lock (_lockObject)
        {
            foreach (var coord in _infectedAgentsCoords)
            {
                // 対応するエージェントが取得出来なかったら以降の処理を行わないで次
                if (!_agents.TryGetValue(coord, out var infectedAgent))
                    continue;

                // 周囲の座標を調べ、感染範囲内にいるエージェントを取得
                bool allNeighborsInfected = true; // 初期状態は全て感染していると仮定
                for (int dx = -_infectionRange; dx <= _infectionRange; dx++)
                {
                    for (int dy = -_infectionRange; dy <= _infectionRange; dy++)
                    {
                        (int, int) neighborCoord = (infectedAgent.X + dx, infectedAgent.Y + dy);

                        // 辞書にその座標のエージェントが存在するか確認
                        if (_agents.ContainsKey(neighborCoord))
                        {
                            if (_agents[neighborCoord].State != AgentState.Infected)
                            {
                                allNeighborsInfected = false;
                                coordsToCheck.Add(neighborCoord); // 感染判定のリストに入れる
                            }
                        }
                    }
                }

                // 全ての隣接エージェントが感染していれば、Skipフラグを設定
                if (allNeighborsInfected)
                {
                    coordsToMarkSkip.Add(coord);
                }
            }

            // ループ後に Skipフラグを設定
            foreach (var coord in coordsToMarkSkip)
            {
                _agents.TryGetValue(coord, out var skipAgent);
                skipAgent.Skip = true;
                _agents[coord] = skipAgent;
            }
        }

        lock (_lockObject)
        {
            _checkAgentCoords.AddRange(coordsToCheck); // lockして同期をとる
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
        
        // 感染確率はジョブ外で計算
        // 基礎感染率×区域補正× (1 + 環境補正) × (1 + 難易度補正) (× (1 - 対象耐性補正))
        float infectionRate = InfectionParameters.BaseRate * (1 + _regionMod) *　(1 + InfectionParameters.EnvMod) * (1 + _difficultyMod);
        
        InfectionJob job = new InfectionJob
        {
            agents = _agentArray,
            infectionRate = infectionRate,
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
        
        // メモリ解放
        _agentArray.Dispose();
        
        return jobHandle;
    }

    [BurstCompile]
    private struct InfectionJob : IJobParallelFor
    {
        public NativeArray<Agent> agents;
        public float infectionRate;
        
        public void Execute(int index)
        {
            Agent agent = agents[index];
            float targetResistMod = agent.Type == AgentType.MagicSoldier ? 0.2f : 0f; // agentsのタイプによってかかる補正
            
            // 感染確率を計算
            // 基礎感染率×区域補正× (1 + 環境補正) × (1 + 難易度補正) × (1 - 対象耐性補正)
            float infectionProbability = infectionRate * (1 - targetResistMod);
            
            // 感染判定。乱数が感染確率
            if (infectionProbability > agent.RandomNumber())
            {
                agent.Infect(); // 感染
            }
            
            agents[index] = agent; // 変更を反映
        }
    }

    #endregion

    #region 死亡判定

    /// <summary>
    /// 死亡判定を行う
    /// </summary>
    private void NearDeathDetermination()
    {
        _nearDeathAgentsArray = new NativeArray<Agent>(_nearDeathAgentsCoords.Count, Allocator.TempJob);
        
        int index = 0;
        foreach (var coord in _nearDeathAgentsCoords)
        {
            if (_agents.TryGetValue(coord, out var agent))
            {
                _nearDeathAgentsArray[index] = agent;
                index++;
            }
        }

        NearDeathJob job = new NearDeathJob
        {
            agents = _nearDeathAgentsArray,
            lethalityRate = InfectionParameters.LethalityRate,
        };
        
        JobHandle jobHandle = job.Schedule(_nearDeathAgentsArray.Length, 64);
        
        jobHandle.Complete();
        
        // 結果を _agents に反映
        for (int i = 0; i < _nearDeathAgentsArray.Length; i++)
        {
            var updatedAgent = _nearDeathAgentsArray[i];
            var coord = (updatedAgent.X, updatedAgent.Y);

            if (_agents.ContainsKey(coord))
            {
                _agents[coord] = updatedAgent; // 更新を反映
            }
        }
        
        // メモリ解放
        _nearDeathAgentsArray.Dispose();
    }
    
    [BurstCompile]
    private struct NearDeathJob : IJobParallelFor
    {
        public NativeArray<Agent> agents;
        public float lethalityRate; // 致死率
        
        public void Execute(int index)
        {
            Agent agent = agents[index];

            if (lethalityRate > agent.RandomNumber())
            {
                agent.NearDeath();
            }
            
            agents[index] = agent; // 変更を反映
        }
    }

    #endregion
    
    /// <summary>
    /// サブツリー含め全てのエージェントを取得する
    /// </summary>
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
                // サブツリー内のエージェントが _agents に含まれていないかチェック
                if (!_agents.ContainsKey((agent.X, agent.Y)))  // 重複を防ぐ
                {
                    yield return agent;
                }
            }
        }
    }
}
