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
        _healthy.text = "";
        _infected.text = "";
        _nearDeath.text = "";
        
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
        _healthy.text = _stateCount.Healthy.ToString();
        _infected.text = _stateCount.Infected.ToString();
        _nearDeath.text = _stateCount.NearDeath.ToString();
    }
}
