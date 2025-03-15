using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class MiniCell
{
    private int _id; // セル自体のID
    
    private MiniQuadtree _quadtree;
    private JobHandle _quadtreeJobHandle;
    public AgentStateCount StateCount { get; private set; }
    
    public MiniCell(int id, int citizen, int magicSoldier)
    {
        _id = id;
        _quadtree = new MiniQuadtree(new Rect(0, 0, 1000, 1000));
        StateCount = new AgentStateCount();
        
        InitializeAgents(citizen, magicSoldier);
    }

    /// <summary>
    /// エージェントの生成
    /// </summary>
    private void InitializeAgents(int citizen, int magicSoldier)
    {
        _quadtree.InitializeAgents(citizen, magicSoldier).Forget();
        
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public void SimulateInfection(float baseInfectionRate, float infectionMultiplier)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        // Quadtree のシミュレーション開始
        _quadtreeJobHandle = _quadtree.SimulateInfection();
        
        // すべての `Quadtree` のジョブが完了するまで待機
        _quadtreeJobHandle.Complete();
        
        stopwatch.Stop();
        
        Debug.Log($"シミュレーション更新:{_id} {stopwatch.ElapsedMilliseconds} ミリ秒");
        // ここでエージェントの状態をカウント
        UpdateStateCount();
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
            if(agent.Type == AgentType.MagicSoldier) StateCount.AddState(1); // 魔法士の数
        }
    }
}
