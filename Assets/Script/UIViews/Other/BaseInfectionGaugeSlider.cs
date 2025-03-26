using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 感染状況を確認するスライダーを管理するクラスのベース
/// </summary>
public abstract class BaseInfectionGaugeSlider : ViewBase
{
    [SerializeField, HighlightIfNull] private Image _healthyGauge;
    [SerializeField, HighlightIfNull] private Image _infectedGauge;
    [SerializeField, HighlightIfNull] private Image _nearDeathGauge;
    
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
    /// Fillを更新する
    /// </summary>
    protected void UpdateGauge(int healthy, int infected, int nearDeath)
    {
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
