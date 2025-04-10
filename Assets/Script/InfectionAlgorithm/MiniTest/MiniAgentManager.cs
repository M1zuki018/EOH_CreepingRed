using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 縮小テスト用のAgentManager(Quadtreeを使わないagentの処理)
/// </summary>
public class MiniAgentManager
{
    // 初期化処理
    private List<UniTask> _generateTasks; // エージェント生成タスクのリスト
    
    // 情報
    private Dictionary<(int x, int y), Agent> _agents; // エージェントの情報
    private readonly float _regionMod; // 感染確率計算の環境補正
    private float _difficultyMod; // 感染確率計算の難易度補正
    private int _infectionRange; // 感染範囲
    private readonly int _maxX = 1000; // セル内の座標の横幅の上限
    
    // シミュレーション関連
    private List<(int, int)> _infectedAgentsCoords; // 処理を行う感染済みのエージェント
    private HashSet<(int, int)> _checkAgentCoords; // 感染判定を行う対象のエージェント
    private List<(int, int)> _nearDeathAgentsCoords = new List<(int, int)>(); // 仮死判定を行う対象のエージェント
    private JobHandle _infectionJobHandle;
    private NativeArray<Agent> _agentArray; // 感染判定を行うエージェントのNativeArray
    private NativeArray<Agent> _nearDeathAgentsArray; // 死亡判定を行うエージェントのNativeArray
    private readonly object _lockObject = new object();
    private HashSet<(int x, int y)> _coordsToMarkSkip = new HashSet<(int x, int y)>(); // Skipフラグを立てるエージェントのリスト

    public MiniAgentManager(float regionMod)
    {
        _regionMod = regionMod;
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

    #region 初期化処理

    /// <summary>
    /// Cellクラスが呼び出す初期化処理
    /// </summary>
    public async UniTask InitializeAgents(int citizen, bool infection = false)
    {
        await GenerateAgents(citizen);
        if(infection) Infection(1);
    }

    /// <summary>
    /// エージェントを感染させる処理
    /// </summary>
    public void Infection(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var coord = (count % _maxX, count % _maxX); // 座標を作成
            _agents.TryGetValue(coord, out Agent targetAgent);
            targetAgent.Infect();
            _agents[coord] = targetAgent; // 感染させたエージェントを辞書に戻す
        }
    }
    
    /// <summary>
    /// agentを並列処理で生成する
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

        // ネイティブ配列からディクショナリに登録
        for (int i = 0; i < agentArray.Length; i++)
        {
            var agent = agentArray[i];
            _agents[(agent.X, agent.Y)] = agent;
        }
        
        agentArray.Dispose();
        
        await UniTask.CompletedTask; // 全てのエージェントの生成を待つ
    }
    
    /// <summary>
    /// agentを生成するJob
    /// </summary>
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

    #region 感染シミュレーション

    // <summary>
    /// 感染状態のエージェントで、Skipフラグがfalseのエージェントを探す
    /// </summary>
    public JobHandle SimulateInfection()
    {
        // リストを最初にクリアしておく
        _infectedAgentsCoords.Clear();
        _checkAgentCoords.Clear();
        _infectionRange = InfectionParameters.InfectionRange; // 感染範囲を更新
        
        IdentifyInfectedAgents();
        if(InfectionParameters.LethalityRate > 0) NearDeathDetermination(); // 致死率が0以上のときは死亡判定を行う
        
        return ProcessInfectionAreas();
    }

    /// <summary>
    /// 感染中のエージェントを特定し、リストに詰める
    /// </summary>
    private void IdentifyInfectedAgents()
    {
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
    }
    
    /// <summary>
    /// 感染範囲内のエージェントを収集し、ジョブをスケジュール
    /// </summary>
    private JobHandle ProcessInfectionAreas()
    {
        var infectionCandidates = CollectInfectionCandidates(); // 感染判定を行うエージェントを収集
        UpdateSkipFlags(); // エージェントのスキップフラグを確認する
    
        lock (_lockObject)
        {
            _checkAgentCoords.AddRange(infectionCandidates);
        }
        
        return PerformInfectionJob();
    }

    /// <summary>
    /// 感染判定を行うエージェントを収集する
    /// </summary>
    private HashSet<(int, int)> CollectInfectionCandidates()
    {
        HashSet<(int, int)> candidates = new();
        _coordsToMarkSkip.Clear();

        foreach (var coord in _infectedAgentsCoords)
        {
            if (!_agents.TryGetValue(coord, out var infectedAgent))
                continue; // _agentsの辞書から、感染を広げる対象が取得できなかったらスキップ

            bool allNeighborsInfected = true; // 感染範囲内の近隣のエージェントが全員感染しているかどうかをチェック
            
            for (int dx = -_infectionRange; dx <= _infectionRange; dx++)
            {
                for (int dy = -_infectionRange; dy <= _infectionRange; dy++)
                {
                    (int, int) neighborCoord = (infectedAgent.X + dx, infectedAgent.Y + dy);
                    
                    if (_agents.TryGetValue(neighborCoord, out var neighbor) && neighbor.State != AgentState.Infected)
                    {
                        allNeighborsInfected = false;
                        candidates.Add(neighborCoord);
                    }
                }
            }
            
            if (allNeighborsInfected)
            {
                _coordsToMarkSkip.Add(coord);
            }
        }
        
        return candidates;
    }
    
    /// <summary>
    /// Skipフラグを更新
    /// </summary>
    private void UpdateSkipFlags()
    {
        foreach (var coord in _coordsToMarkSkip)
        {
            if (_agents.TryGetValue(coord, out var agent))
            {
                agent.Skip = true;
                _agents[coord] = agent;
            }
        }
    }
    
    /// <summary>
    /// 感染判定を並列処理で一斉に行う
    /// </summary>
    private JobHandle PerformInfectionJob()
    {
        _agentArray = new NativeArray<Agent>(_checkAgentCoords.Count, Allocator.TempJob);
        
        // エージェントを辞書からNativeArrayに変換
        int index = 0;
        foreach (var coord in _checkAgentCoords)
        {
            if (_agents.TryGetValue(coord, out var agent))
            {
                _agentArray[index++] = agent;
            }
        }
        
        // 感染確率はジョブ外で計算
        // 基礎感染率×区域補正× (1 + 環境補正) × (1 + 難易度補正) (× (1 - 対象耐性補正))
        float infectionRate = InfectionParameters.BaseRate * (1 + _regionMod) *　(1 + InfectionParameters.EnvMod) * (1 + _difficultyMod);
        
        InfectionJob job = new InfectionJob
        {
            agents = _agentArray,
            infectionRate = infectionRate,
            random = new Unity.Mathematics.Random((uint)Random.Range(1, int.MaxValue))
        };
        
        JobHandle jobHandle = job.Schedule(_agentArray.Length, 64);
        jobHandle.Complete();
        
        // 結果を _agents に反映
        for (int i = 0; i < _agentArray.Length; i++)
        {
            var updatedAgent = _agentArray[i];
            _agents[(updatedAgent.X, updatedAgent.Y)] = updatedAgent; // 更新を反映
        }
        
        _agentArray.Dispose();
        return jobHandle;
    }

    [BurstCompile]
    private struct InfectionJob : IJobParallelFor
    {
        public NativeArray<Agent> agents;
        public float infectionRate;
        public Unity.Mathematics.Random random;
        
        public void Execute(int index)
        {
            Agent agent = agents[index];
            float targetResistMod = agent.Type == AgentType.MagicSoldier ? 0.2f : 0f; // agentsのタイプによってかかる補正
            
            // 感染確率を計算
            // 基礎感染率×区域補正× (1 + 環境補正) × (1 + 難易度補正) × (1 - 対象耐性補正)
            float infectionProbability = infectionRate * (1 - targetResistMod);
            
            // 感染判定。乱数が感染確率
            if (infectionProbability > random.NextInt(100))
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
                _nearDeathAgentsArray[index++] = agent;
            }
        }

        NearDeathJob job = new NearDeathJob
        {
            agents = _nearDeathAgentsArray,
            lethalityRate = InfectionParameters.LethalityRate,
            random =  new Unity.Mathematics.Random((uint)Random.Range(1, int.MaxValue))
        };
        
        JobHandle jobHandle = job.Schedule(_nearDeathAgentsArray.Length, 64);
        jobHandle.Complete();
        
        // 結果を _agents に反映
        for (int i = 0; i < _nearDeathAgentsArray.Length; i++)
        {
            var updatedAgent = _nearDeathAgentsArray[i];
            _agents[(updatedAgent.X, updatedAgent.Y)] = updatedAgent; // 更新を反映
        }
        
        // メモリ解放
        _nearDeathAgentsArray.Dispose();
    }
    
    [BurstCompile]
    private struct NearDeathJob : IJobParallelFor
    {
        public NativeArray<Agent> agents;
        public float lethalityRate; // 致死率
        public Unity.Mathematics.Random random;
        
        public void Execute(int index)
        {
            Agent agent = agents[index];

            if (lethalityRate > random.NextInt(100))
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
        // 自分のエージェントを最初に返す
        foreach (var agent in _agents.Values)
        {
            yield return agent;
        }
    }
}
