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

    public override UniTask OnBind()
    {
        Grid.StateUpdated += FillUpdate; // ステートカウントの更新イベントを登録
        return base.OnBind();
    }

    private void OnDestroy()
    {
        Grid.StateUpdated -= FillUpdate;
    }

    /// <summary>
    /// Fillを更新する
    /// </summary>
    private void FillUpdate(int healthy, int infected, int nearDeath)
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
        float healthyFill = (float)healthy / sum;
        float infectedFill = (float)infected / sum;
        float nearDeathFill = (float)nearDeath / sum;

        // ゲージに反映する
        _healthyGauge.fillAmount = Mathf.Clamp01(healthyFill + infectedFill + nearDeathFill);
        _infectedGauge.fillAmount = Mathf.Clamp01(infectedFill + nearDeathFill);
        _nearDeathGauge.fillAmount = Mathf.Clamp01(nearDeathFill);
    }
}
