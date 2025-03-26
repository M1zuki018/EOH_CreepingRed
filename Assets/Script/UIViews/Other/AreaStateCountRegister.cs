using System;
using System.Collections.Generic;

public class AreaStateCountRegister
{
    private static AreaStateCountRegister _instance;
    public static AreaStateCountRegister Instance => _instance ??= new AreaStateCountRegister();
    
    // 各エリアのenumとカウントクラスのPairのディクショナリ
    private Dictionary<SectionEnum, AgentStateCount> _stateCountDictionary = new Dictionary<SectionEnum, AgentStateCount>();
    private SectionEnum _currentSection;   
    private AreaStateCountRegister() {} // 外部からのインスタンス化を防ぐ
    
    public event Action OnUpdate; // 数値が変わったタイミングで発火するようにする

    /// <summary>
    /// エリアの辞書を作成する
    /// </summary>
    public void RegisterArea(SectionEnum section, AgentStateCount agentStateCount, Area area)
    {
        _stateCountDictionary[section] = agentStateCount;
        area.StateUpdated += HandleUpdate;
    }

    private void HandleUpdate() => OnUpdate?.Invoke();
    
    /// <summary>
    /// 引数に対応したAgentStateCountを返す
    /// </summary>
    public AgentStateCount GetStateCount(SectionEnum section)
    {
        return _stateCountDictionary[section];
    }
}
