using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    // 定数
    private const int MAX_AGENTS = 10000; // 1ツリーの最大エージェント数
    private const int MAX_DEPTH = 10; // 最大分割回数
    
    // QuadTree関連
    private Dictionary<MiniQuadtree, bool> _subTrees; // サブツリーとSkipフラグ
    private Dictionary<(int x, int y), Agent> _agents; // エージェントの情報
    private Rect _bounds;
    private int _depth = 0; // 現在の分割数
    private int _maxX = 1000; // セル内の座標の横幅の上限
    
    // シミュレーション関連
    private List<(int, int)> _infectedAgentsCoords = new List<(int, int)>(); // 処理を行う感染済みのエージェント
    private HashSet<(int, int)> _checkAgentCoords = new HashSet<(int, int)>(); // 感染判定を行う対象のエージェント
    private JobHandle _infectionJobHandle;
    private NativeArray<Agent> _agentArray;
    private readonly object _subdivideLock = new object();  // ロックオブジェクト
    private float _regionMod; // 感染確率計算の環境補正
    private float _difficultyMod; // 感染確率計算の難易度補正

    public MiniQuadtree(Rect bounds, float regionMod, int depth = 0)
    {
        _bounds = bounds;
        _regionMod = regionMod;
        _depth = depth;
        _subTrees = new Dictionary<MiniQuadtree, bool>();
        _agents = new Dictionary<(int x, int y), Agent>();
        _infectedAgentsCoords = new List<(int, int)>();
        _checkAgentCoords = new HashSet<(int, int)>();
        SetDifficultyMod();
    }

    /// <summary>
    /// 現在の難易度に応じて難易度補正の値を決める
    /// </summary>
    private void SetDifficultyMod()
    {
        _difficultyMod = GameSettingsManager.Difficulty switch
        {
            DifficultyEnum.Breeze => 0,
            DifficultyEnum.Storm => 0.05f, // 普通
            DifficultyEnum.Catastrophe => 0.1f, //難しい
            DifficultyEnum.Unknown => 0.2f, // 激ムズ
            DifficultyEnum.Custom => 0f, // カスタム難易度
        };
    }

    #region 初期化
    /// <summary>
    /// シミュレーションの初期化処理
    /// </summary>
    public async UniTaskVoid InitializeAgents(int citizen, int magicSoldier)
    {
        Debug.Log($"受け取った数：一般市民{citizen} 魔法士{magicSoldier}");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        await GenerateAgents(citizen, magicSoldier);
        
        stopwatch.Stop();
        
        Debug.Log($"生成完了（実行時間:{stopwatch.ElapsedMilliseconds}ミリ秒） エージェントの数{_agents.Count}/サブツリーの数{_subTrees.Count}");
        
        _agents.TryGetValue((0,0), out var test);
        test.Infect();
        _agents[(0, 0)] = test;
    }

    private async UniTask GenerateAgents(int citizen, int magicSoldier)
    {
        int i = 0;
        // エージェントを並列処理で生成
        for (; i < citizen; i++)
        {
            // 座標の計算（必要に応じてロジックを変更）
            int x = i % _maxX;  // 例：X座標の計算
            int y = i / _maxX;  // 例：Y座標の計算

            _agents[(x, y)] = new Agent(i, AgentType.Citizen, x, y);
        
            // クワッドツリーに追加（非同期で追加）
            Insert(_agents[(x, y)]);
        }
        
        // エージェントを並列処理で生成
        for (; i < citizen + magicSoldier; i++)
        {
            // 座標の計算（必要に応じてロジックを変更）
            int x = i % _maxX;  // 例：X座標の計算
            int y = i / _maxX;  // 例：Y座標の計算

            _agents[(x, y)] = new Agent(i, AgentType.MagicSoldier, x, y);
        
            // クワッドツリーに追加（非同期で追加）
            Insert(_agents[(x, y)]);
        }
        
        await UniTask.Yield();
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

        if (_agents.Count < MAX_AGENTS)
        {
            _agents[(agent.X, agent.Y)] = agent; // 容量に空きがあったらそのままエージェントを追加
            return;
        }

        if (_depth >= MAX_DEPTH) // 分割数に余裕がない場合
        {
            _agents[(agent.X, agent.Y)] = agent; // そのままエージェントを追加
            return;
        }
        
        lock (_subdivideLock)  // 排他制御を行って分割処理をシリアルに実行
        {
            if (_subTrees.Count == 0)  // 既にサブツリーが作成されていない場合のみ分割
            {
                Subdivide(); // 分割処理
            }
        }
    }

    /// <summary>
    /// サブツリーを分割する処理
    /// </summary>
    private void Subdivide() 
    {
        Debug.Log("サブツリー生成");
        
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

    #region 感染処理

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
            baseRate = 90f,
            regionMod = 2
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
        public float baseRate; // 基礎感染率（スキルツリーによって変動）
        public float regionMod; // 区域補正（AreaData参照。変動しない）
        public float envMod; // 環境補正（ゲーム進行中に変動する）
        public float difficultyMod; // 難易度補正（GameSettingsの難易度参照。変動しない）
        
        public void Execute(int index)
        {
            Agent agent = agents[index];
            float targetResistMod = agent.Type == AgentType.MagicSoldier ? 0.2f : 0f; // agentsのタイプによってかかる補正
            envMod = 0;
            difficultyMod = 0;
            
            // 感染確率を計算
            // 基礎感染率×区域補正× (1 + 環境補正) × (1 + 難易度補正) × (1 - 対象耐性補正)
            float infectionProbability = baseRate * (1 + regionMod) * (1 + envMod) * (1 + difficultyMod) * (1 - targetResistMod);
            
            // 感染判定。乱数が感染確率
            if (infectionProbability > agent.RandomNumber())
            {
                agent.Infect(); // 感染
            }
            
            agents[index] = agent; // 変更を反映
        }
    }

    #endregion
}
