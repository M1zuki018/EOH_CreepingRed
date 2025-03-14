using UnityEngine;

/// <summary>
/// エージェント約10万人が詰められたセル
/// </summary>
public class Cell
{
    private int _id;

    public Cell(int id)
    {
        _id = id;
    }
    
    public void InitializeAgents(Area area)
    {
        // ここでエージェントをランダム配置
        int agentsPerCell = area.Population / 100000; // 10万人単位
        for (int i = 0; i < agentsPerCell; i++)
        {
            // エージェントを配置（ランダム）
            //var agent = new Agent(i, AgentType.Citizen, X, Y);
            var agent = new Agent(i, AgentType.Citizen, 0, 0);
            area.Agents.Add(agent);
        }
        Debug.Log($"{area.Name.ToString()}Cell Initialize Finish");
    }
}
