using System;
using Random = UnityEngine.Random;

/// <summary>
/// 1エージェントのクラス
/// </summary>
public class Agent
{
    public int Id { get; }
    public AgentType Type { get; }
    public NormalAgentState State { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    //private int random = new Random();

    public Agent(int id, AgentType type, int x, int y)
    {
        Id = id;
        Type = type;
        X = x;
        Y = y;
        State = NormalAgentState.Healthy;
    }

    public void Move(Grid grid)
    {
        // 生活・逃走などのルールに応じた移動処理
        // int dx = random.Next(-1, 2);
        // int dy = random.Next(-1, 2);
        // X = Math.Max(0, Math.Min(grid.Cells.GetLength(0) - 1, X + dx));
        // Y = Math.Max(0, Math.Min(grid.Cells.GetLength(1) - 1, Y + dy));
    }

    public void UpdateState(Grid grid)
    {
        // 感染拡大・停止の処理
        Area currentCell = grid.GetArea(X, Y);
        if (State == NormalAgentState.Healthy && currentCell.InfectionRisk > 0.5)
        {
            // if (random.NextDouble() < currentCell.InfectionRisk)
            // {
            //     State = NormalAgentState.Infected;
            // }
        }
    }
}