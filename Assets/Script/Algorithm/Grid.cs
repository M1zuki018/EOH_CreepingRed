using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 感染シミュレーション全体を管理するクラス
/// </summary>
public class Grid
{
    private readonly int _width; // ヨコ
    public int Width => _width;
    private readonly int _height; // タテ
    public int Height => _height;

    public Area[,] Areas { get; } // エリアを動的に変更する予定がないので二次元配列でやってみる

    public Grid(List<AreaSettingsSO> areaSettings)
    {
        // AreaSettingsSO の X, Y に基づいてエリアを配置
        foreach (var areaSetting in areaSettings)
        {
            int x = areaSetting.x;
            int y = areaSetting.y;

            // エリアを配置
            Areas[x, y] = new Area(areaSetting);
            Debug.Log($"Area placed at ({x}, {y}) : {areaSetting.name.ToString()}");
        }
        
        Debug.Log($"Grid Initialize Finish");
    }

    /// <summary>
    /// エリアデータを取得する（範囲外なら null）
    /// </summary>
    public Area? GetArea(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
        {
            return null; // 範囲外なら null を返す
        }
        return Areas[x, y];
    }
}
