using System;

/// <summary>
/// 感染シミュレーション全体を管理するクラス
/// </summary>
public class Grid
{
    private int width, height;
    public Area[,] Cells { get; }

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
        Cells = new Area[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cells[x, y] = new Area(x, y);
            }
        }
    }

    public Area GetArea(int x, int y)
    {
        return Cells[Math.Max(0, Math.Min(x, width - 1)), Math.Max(0, Math.Min(y, height - 1))];
    }
}
