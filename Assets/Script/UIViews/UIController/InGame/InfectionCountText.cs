using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 感染のテキストを管理するクラス
/// </summary>
public class InfectionCountText : ViewBase
{
    [SerializeField, HighlightIfNull] private Text _healthy;
    [SerializeField, HighlightIfNull] private Text _infected;
    [SerializeField, HighlightIfNull] private Text _nearDeath;
    private AgentStateCount _stateCount;
    
    public override UniTask OnUIInitialize()
    {
        _healthy.text = Formatted(0);
        _infected.text = Formatted(0);
        _nearDeath.text = Formatted(0);
        
        return base.OnUIInitialize();
    }

    /// <summary>
    /// AgentStateCountクラスの参照をセットする
    /// </summary>
    public void SetAgentStateCount(AgentStateCount agentStateCount)
    {
        _stateCount = agentStateCount;
    }

    /// <summary>
    /// UIを更新する
    /// </summary>
    public void CountUpdate()
    {
        _healthy.text = Formatted(_stateCount.Healthy);
        _infected.text = Formatted(_stateCount.Infected);
        _nearDeath.text = Formatted(_stateCount.NearDeath);
    }

    /// <summary>
    /// 表示のフォーマットを整える
    /// </summary>
    private string Formatted(int num)
    {
        return num.ToString("00,000,000"); // 8桁になるようにゼロ埋めしつつカンマ区切り
    }
}
