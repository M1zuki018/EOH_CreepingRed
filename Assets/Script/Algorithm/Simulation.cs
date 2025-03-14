using System;
using System.Collections.Generic;

/// <summary>
/// 1日のサイクル
/// </summary>
public class Simulation
{
    private Grid _grid;
    private int day = 0;
    private int timeStep = 0;
    private int maxDays = 30;  // 1ヶ月

    public Simulation(List<AreaSettingsSO> areaSettings)
    {
        _grid = new Grid(areaSettings); // グリッドを生成する
    }

    public void Run()
    {
        while (day < maxDays)
        {
            Console.WriteLine($"Day {day + 1}");

            for (int step = 0; step < 12; step++) // 1日12回更新
            {
                UpdateSimulation();
                timeStep++;
            }

            day++;
        }
    }

    private void UpdateSimulation()
    {
        
    }

    private void HandleBattles()
    {
        // 魔法士と亡霊の戦闘ロジックを記述
    }
}
