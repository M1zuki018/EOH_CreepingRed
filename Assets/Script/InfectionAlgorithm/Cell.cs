using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class Cell
{
    private int _id; // セル自体のID
    
    private Quadtree _quadtree;
    public AgentStateCount StateCount { get; private set; }
    
    public Cell(int id, int citizen, int magicSoldier)
    {
        _id = id;
        _quadtree = new Quadtree(new Rect(0, 0, 1000, 1000));
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
    /// 各状態のエージェントの数を集計する
    /// </summary>
    private void UpdateStateCount(int magicSoldier)
    {
        /*
        CellStateCount.ResetStateCount();
        foreach (var agent in _agents)
        {
            CellStateCount.AddState(agent.State); // 各ステート
            CellStateCount.AddState(magicSoldier); // 魔法士の数
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
