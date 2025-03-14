using System;
using System.Collections.Generic;

/// <summary>
/// 1日のサイクル
/// </summary>
public class Simulation
{
    private Grid grid;
    private List<Agent> agents;
    private int day = 0;
    private int timeStep = 0;
    private int maxDays = 30;  // 1ヶ月

    public Simulation(int width, int height, int agentCount)
    {
        grid = new Grid(width, height);
        agents = new List<Agent>();
        Random random = new Random();

        // エージェントをランダム配置
        for (int i = 0; i < agentCount; i++)
        {
            int x = random.Next(width);
            int y = random.Next(height);
            AgentType type = (i % 10 == 0) ? AgentType.MagicSoldier : AgentType.Citizen;
            agents.Add(new Agent(i, type, x, y));
        }
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
        foreach (var row in grid.Cells)
        {
            row.UpdateEnvironment();
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
