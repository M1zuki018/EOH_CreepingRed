/// <summary>
/// 感染シミュレーション全体を管理するクラス
/// </summary>
public class Grid
{
    private readonly int _width; // ヨコ
    private readonly int _height; // タテ

    public Area[,] Areas { get; } // エリアを動的に変更する予定がないので二次元配列でやってみる

    public Grid(int width, int height)
    {
        _width = width;
        _height = height;
        
        Areas = new Area[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Areas[x, y] = new Area(x, y);
            }
        }
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
