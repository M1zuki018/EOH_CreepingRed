using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class Cell
{
    private int _id;

    private List<Agent> _agents { get; } = new List<Agent>();
    private List<Agent> _magicAgents { get; } = new List<Agent>();
    
    public Cell(int id, int citizen, int magicSoldier)
    {
        _id = id;
        InitializeAgents(citizen, magicSoldier);
    }
    
    /// <summary>
    /// 初期化処理：エージェントを生成する
    /// </summary>
    private void InitializeAgents(int citizen, int magicSoldier)
    {
        for (int i = 0; i < citizen; i++)
        {
            var agent = new Agent(i, AgentType.Citizen, 0, 0);
            _agents.Add(agent);
        }

        for (int i = 0; i < magicSoldier; i++)
        {
            var agent = new Agent(i, AgentType.MagicSoldier, 0, 0);
            _magicAgents.Add(agent);
        }
        
        Debug.Log($"{_id} : 一般市民{_agents.Count} 魔法士{_magicAgents.Count}");
    }
}
