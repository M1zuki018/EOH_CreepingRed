using System.Collections.Generic;

public class AreaStateCountRegister
{
    private static AreaStateCountRegister _instance;
    public static AreaStateCountRegister Instance => _instance ??= new AreaStateCountRegister();
    
    // 各エリアのenumとカウントクラスのPairのディクショナリ
    private Dictionary<SectionEnum, AgentStateCount> _stateCountDictionary = new Dictionary<SectionEnum, AgentStateCount>();
    
    private AreaStateCountRegister() {} // 外部からのインスタンス化を防ぐ

    /// <summary>
    /// エリアの辞書を作成する
    /// </summary>
    public void RegisterArea(SectionEnum section, AgentStateCount agentStateCount)
    {
        _stateCountDictionary[section] = agentStateCount;
    }
}
