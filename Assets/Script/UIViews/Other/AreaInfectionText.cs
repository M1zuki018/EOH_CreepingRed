using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// エリアの感染状況を表すテキストを管理するクラス
/// </summary>
public class AreaInfectionText : ViewBase
{
    [SerializeField, HighlightIfNull] private Text _healthy;
    [SerializeField, HighlightIfNull] private Text _infected;
    [SerializeField, HighlightIfNull] private Text _nearDeath;
    private AgentStateCount _stateCount;
    
    public override UniTask OnUIInitialize()
    {
        ResetCount();
        return base.OnUIInitialize();
    }

    public override UniTask OnBind()
    {
        AreaStateCountManager.Instance.OnUpdate += CountUpdate;
        return base.OnBind();
    }
    
    private void OnDestroy()
    {
        AreaStateCountManager.Instance.OnUpdate -= CountUpdate;
    }

    /// <summary>
    /// カウントをリセットする
    /// </summary>
    private void ResetCount()
    {
        _healthy.text = Formatted(0);
        _infected.text = Formatted(0);
        _nearDeath.text = Formatted(0);
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
        if (_stateCount == null)
        {
            ResetCount();
            return;
        }

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
