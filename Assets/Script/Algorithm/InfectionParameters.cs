/// <summary>
/// 感染シミュレーションに使用する変数
/// </summary>
public static class InfectionParameters
{
    /// <summary>基礎感染率（拡散性を足していく）</summary>
    public static float BaseRate = 20f;

    /// <summary>環境補正</summary>
    public static float EnvMod = 0f;

    /// <summary>致死率</summary>
    public static float LethalityRate = 10f;

    /// <summary>感染範囲</summary>
    public static int InfectionRange = 2;
}
