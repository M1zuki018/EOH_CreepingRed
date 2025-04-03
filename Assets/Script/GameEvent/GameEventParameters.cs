using R3;

/// <summary>
/// ゲーム進行によるイベントに関連するパラメーター
/// </summary>
public static class GameEventParameters
{
    /// <summary>解放ポイント</summary>
    public static ReactiveProperty<int> Resource = new ReactiveProperty<int>(100);
    
    /// <summary>発覚率</summary>
    public static int DetectionRate;
}
