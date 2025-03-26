using System;
using Cysharp.Threading.Tasks;

/// <summary>
/// エリアの感染スライダーを管理するクラス（Areaクラスとセット）
/// </summary>
public class AreaInfectionGauge : InfectionGaugeSliderBase
{
    private AgentStateCount _stateCount;

    public override UniTask OnBind()
    {
        AreaStateCountManager.Instance.OnUpdate += FillUpdate;
        return base.OnBind();
    }

    private void OnDestroy()
    {
        AreaStateCountManager.Instance.OnUpdate -= FillUpdate;
    }

    /// <summary>
    /// AgentStateCountクラスの参照をセットする
    /// </summary>
    public void SetAgentStateCount(AgentStateCount agentStateCount)
    {
        _stateCount = agentStateCount;
    }

    /// <summary>
    /// Fillを更新する
    /// </summary>
    public void FillUpdate()
    {
        if(_stateCount == null) return;
        
        int healthy = _stateCount.Healthy;
        int infected = _stateCount.Infected;
        int nearDeath = _stateCount.NearDeath;
        
        UpdateGauge(healthy, infected, nearDeath); // 基底クラスのゲージ更新処理を呼ぶ
    }
}
