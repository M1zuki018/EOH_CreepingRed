using System;
using System.Collections.Generic;

/// <summary>
/// 1日のサイクル
/// </summary>
public class Simulation
{
    private Grid grid;
    public Grid Grid => grid;
    private List<Agent> agents = new List<Agent>();
    private int day = 0;
    private int timeStep = 0;
    private int maxDays = 30;  // 1ヶ月

    public Simulation(List<AreaSettingsSO> areaSettings)
    {
        grid = new Grid(areaSettings); // グリッドを生成する
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
        // 環境更新
        foreach (var row in grid.Areas)
        {
            row.UpdateEnvironment(10, 10,10);
        }

        // エージェント更新
        foreach (var agent in agents)
        {
            agent.Move(grid);
            agent.UpdateState(grid);
        }

        // 戦闘・イベント処理（魔法士 vs 亡霊など）
        HandleBattles();

        Console.WriteLine($"TimeStep: {timeStep}");
    }

    private void HandleBattles()
    {
        // 魔法士と亡霊の戦闘ロジックを記述
    }
}
