using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エージェント約10万人が詰められたセル
/// </summary>
public class Cell
{
    private int _id;
    
    public List<Agent> Agents { get; } = new List<Agent>();
    
    public Cell(int id, int population)
    {
        _id = id;
        InitializeAgents(population);
    }
    
    public void InitializeAgents(int population)
    {
        for (int i = 0; i < population; i++)
        {
            var agent = new Agent(i, AgentType.Citizen, 0, 0);
            Agents.Add(agent);
        }
        Debug.Log($"{_id} : {Agents.Count}");
    }
}
