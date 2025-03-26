using System;
using System.Collections.Generic;

/// <summary>
/// AreaStateCountクラスを収集/管理し、UIへデータを渡すためのクラス
/// </summary>
public class AreaStateCountRegister
{
    private static AreaStateCountRegister _instance;
    public static AreaStateCountRegister Instance => _instance ??= new AreaStateCountRegister();
    
    // 各エリアのenumとカウントクラスのPairのディクショナリ
    private readonly Dictionary<SectionEnum, AgentStateCount> _stateCountDictionary = new Dictionary<SectionEnum, AgentStateCount>();
    public event Action OnUpdate; // 数値が変わったタイミングで発火するようにする
    
    private AreaStateCountRegister() {} // 外部からのインスタンス化を防ぐ

    /// <summary>
    /// エリアの辞書を作成する
    /// </summary>
    public void RegisterArea(SectionEnum section, AgentStateCount agentStateCount, Area area)
    {
        if (_stateCountDictionary.ContainsKey(section))
        {
            area.StateUpdated -= HandleUpdate; // 重複登録を防ぐため解除
        }
        
        _stateCountDictionary[section] = agentStateCount;
        area.StateUpdated += HandleUpdate;
    }
    
    /// <summary>
    /// 指定されたセクションに対応する AgentStateCount を取得する
    /// </summary>
    public bool GetStateCount(SectionEnum section, out AgentStateCount stateCount)
    {
        return _stateCountDictionary.TryGetValue(section, out stateCount);
    }
    
    private void HandleUpdate() => OnUpdate?.Invoke();
}
