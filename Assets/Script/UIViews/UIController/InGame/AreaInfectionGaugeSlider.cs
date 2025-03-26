using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// エリアの感染スライダーを管理するクラス（Areaクラスとセット）
/// </summary>
public class AreaInfectionGaugeSlider : ViewBase
{
    [SerializeField, HighlightIfNull] private Image _healthyGauge;
    [SerializeField, HighlightIfNull] private Image _infectedGauge;
    [SerializeField, HighlightIfNull] private Image _nearDeathGauge;
    private AgentStateCount _stateCount;
    
    public override UniTask OnUIInitialize()
    {
        ResetGauge(); // スライダーの初期化
        return base.OnUIInitialize();
    }
    
    /// <summary>
    /// スライダーの初期化
    /// </summary>
    private void ResetGauge()
    {
        _healthyGauge.fillAmount = 1;
        _infectedGauge.fillAmount = 0;
        _nearDeathGauge.fillAmount = 0;
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
        
        var sum = healthy + infected + nearDeath; // 合計を求める
     
        if (sum <= 0f)
        {
            ResetGauge(); // すべて0の場合、ゲージをリセット
            return;
        }
        
        // それぞれのFillの幅を計算
        float infectedFill = (float)infected / sum;
        float nearDeathFill = (float)nearDeath / sum;

        // ゲージに反映する(Healthyは常に1のままにしておく)
        _infectedGauge.fillAmount = Mathf.Clamp01(infectedFill + nearDeathFill);
        _nearDeathGauge.fillAmount = Mathf.Clamp01(nearDeathFill);
    }
}
