using System.Collections.Generic;

/// <summary>
/// 各区域を管理するクラス
/// </summary>
public class Area
{
    public int X { get; }
    public int Y { get; }
    public List<Agent> Agents { get; } = new List<Agent>();
    public bool HasGhost { get; set; }  // 亡霊がいるか
    public double InfectionRisk { get; set; }  // 感染リスク

    public Area(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void UpdateEnvironment()
    {
        // 感染リスクの変動や環境変化の処理
    }
}
