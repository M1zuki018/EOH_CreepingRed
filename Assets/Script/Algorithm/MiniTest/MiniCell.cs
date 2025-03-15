using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class MiniCell
{
    private int _id; // セル自体のID
    
    private MiniQuadtree _quadtree;
    public AgentStateCount StateCount { get; private set; }
    
    public MiniCell(int id, int citizen, int magicSoldier)
    {
        _id = id;
        _quadtree = new MiniQuadtree(new Rect(0, 0, 1000, 1000));
        StateCount = new AgentStateCount();
        
        InitializeAgents(citizen, magicSoldier).Forget();
    }

    /// <summary>
    /// エージェントの生成
    /// </summary>
    private async UniTaskVoid InitializeAgents(int citizen, int magicSoldier)
    {
        _quadtree.InitializeAgents(citizen, magicSoldier).Forget();
    }

    /// <summary>
    /// 各状態のエージェントの数を集計する
    /// </summary>
    private void UpdateStateCount(int magicSoldier)
    {
        /*
        StateCount.ResetStateCount();
        foreach (var agent in _agents)
        {
            StateCount.AddState(agent.State); // 各ステート
            StateCount.AddState(magicSoldier); // 魔法士の数
        }
        */
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public void SimulateInfection(float baseInfectionRate, float infectionMultiplier)
    {
        _quadtree.SimulateInfection();
    }
}
