/// <summary>
/// 各状態のエージェントの数を管理するクラス
/// </summary>
public class AgentStateCount
{
    public int Healthy {get; private set;}
    public int Infected {get; private set;}
    public int NearDeath {get; private set;}
    public int Ghost{get; private set;}
    public int Perished {get; private set;}
    public int MagicSoldiers{get; private set;}

    /// <summary>
    /// 各状態のエージェントを追加
    /// </summary>
    public void AddState(AgentState state)
    {
        switch (state)
        {
            case AgentState.Healthy:
                Healthy++;
                break;
            case AgentState.Infected:
                Infected++;
                break;
            case AgentState.NearDeath:
                NearDeath++;
                break;
            case AgentState.Ghost:
                Ghost++;
                break;
            case AgentState.Perished:
                Perished++;
                break;
        }
    }

    /// <summary>
    /// 魔法士エージェントを追加
    /// </summary>
    public void AddState(int magicSoldiers)
    {
        MagicSoldiers += magicSoldiers;
    }

    /// <summary>
    /// 値を更新する
    /// </summary>
    public void UpdateStateCount(int health, int infected, int nearDeath, int ghost, int perished, int magicSoldiers)
    {
        Healthy += health;
        Infected += infected;
        NearDeath += nearDeath;
        Ghost += ghost;
        Perished += perished;
        MagicSoldiers += magicSoldiers;
    }

    public void ResetStateCount()
    {
        Healthy = 0;
        Infected = 0;
        NearDeath = 0;
        Ghost = 0;
        Perished = 0;
        MagicSoldiers = 0;
    }
}
