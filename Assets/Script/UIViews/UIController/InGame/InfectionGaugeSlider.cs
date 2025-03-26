using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 感染スライダーを管理するクラス
/// </summary>
public class InfectionGaugeSlider : ViewBase
{
    [SerializeField, HighlightIfNull] private Image _healthyGauge;
    [SerializeField, HighlightIfNull] private Image _infectedGauge;
    [SerializeField, HighlightIfNull] private Image _nearDeathGauge;

    public override UniTask OnUIInitialize()
    {
        // スライダーの初期化
        _healthyGauge.fillAmount = 1;
        _infectedGauge.fillAmount = 0;
        _nearDeathGauge.fillAmount = 0;
        
        return base.OnUIInitialize();
    }

    /// <summary>
    /// Fillを更新する
    /// </summary>
    public void FillUpdate(float healthy, float infected, float nearDeath)
    {
        var sum = healthy + infected + nearDeath; // 合計を求める
     
        if (sum <= 0f)
        {
            // すべて0の場合、ゲージをリセット
            _healthyGauge.fillAmount = 1f;
            _infectedGauge.fillAmount = 0f;
            _nearDeathGauge.fillAmount = 0f;
            return;
        }
        
        // それぞれのFillの幅を計算
        float healthyFill = healthy / sum;
        float infectedFill = infected / sum;
        float nearDeathFill = nearDeath / sum;

        // ゲージに反映する
        _healthyGauge.fillAmount = Mathf.Clamp01(healthyFill);
        _infectedGauge.fillAmount = Mathf.Clamp01(healthyFill + infectedFill);
        _nearDeathGauge.fillAmount = Mathf.Clamp01(healthyFill + infectedFill + nearDeathFill);
    }
}
