/// <summary>
/// 1エージェントの構造体
/// </summary>
public struct Agent
{
    public int Id { get; }
    public AgentType Type { get; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public AgentState State { get; private set; }
    public bool Skip{ get; set;}

    public Agent(int id, AgentType type, int x, int y)
    {
        Id = id;
        Type = type;
        X = x;
        Y = y;
        State = AgentState.Healthy;
        Skip = false;
    }

    /*
    public void Move(Grid grid)
    {
        // 生活・逃走などのルールに応じた移動処理
        int dx = _random.NextInt(-1, 2);
        int dy = _random.NextInt(-1, 2);
        X = Math.Max(0, Math.Min(grid.Areas.GetLength(0) - 1, X + dx));
        Y = Math.Max(0, Math.Min(grid.Areas.GetLength(1) - 1, Y + dy));
    }
    */
    
    /// <summary>
    /// 感染
    /// </summary>
    public void Infect()
    {
        State = AgentState.Infected;
    }

    /// <summary>
    /// 仮死
    /// </summary>
    public void NearDeath()
    { 
        State = AgentState.NearDeath;
    }
}