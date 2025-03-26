using Cysharp.Threading.Tasks;

/// <summary>
/// 都市全体の感染スライダーを管理するクラス（Gridクラスとセット）
/// </summary>
public class CityInfectionGauge : InfectionGaugeSliderBase
{
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
        UpdateGauge(healthy, infected, nearDeath); // 基底クラスのゲージ更新処理を呼ぶ
    }
}
