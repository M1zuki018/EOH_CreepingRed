using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

/// <summary>
/// 縮小テスト用各区域を管理するクラス
/// </summary>
public class MiniArea
{
    #region パラメータ

    // 基本情報
    public int X { get; } // 座標
    public int Y { get; }
    public SectionEnum Name { get; } // 名称
    public AreaCategoryEnum Category { get; } // 区域の区分
    public int Population { get; private set; }  // 総人口（万人）
    public int CitizenPopulation { get; private set; }  // 一般市民人口
    public int MagicSoldierPopulation { get; private set; }     // 魔法士人口
    public float AreaSize { get; } // 面積(㎢)
    public int PopulationDensity { get; } // 人口密度(人/㎢)
    
    // 状態パラメータ
    public int Security { get; private set; }  // 治安（0-100）
    public int MobilityRate { get; private set; } // 移動率（0-100）
    public int InfectionRate { get; private set; } // 感染成功率（%）
    public int Control { get; private set; } // 統制力（0-100）

    // 特殊フラグ（条件）
    public List<string> SpecialFlags { get; } 

    // 動的データ
    public int Healthy { get; private set; } // 健康な人
    public int Infected { get; private set; } // 感染者
    public int NearDeath { get; private set; } // 仮死者
    public int Ghost { get; private set; } // 亡霊
    public int Perished { get; private set; } // 完全死亡者

    #endregion
    
    private List<MiniCell> _cells = new List<MiniCell>();
    public AgentStateCount AreaStateCount { get; private set; }

    /// <summary>
    /// エリアのコンストラクタ
    /// </summary>
    public MiniArea(AreaSettingsSO settings)
    {
        X = settings.X;
        Y = settings.Y;
        Name = settings.Name;
        Category = settings.Category;
        Population = settings.Population * 1000;
        CitizenPopulation = settings.CitizenPopulation * 1000;
        MagicSoldierPopulation = settings.MagicSoldierPopulation * 1000;
        AreaSize = settings.AreaSize;
        PopulationDensity = settings.PopulationDensity;
        Security = settings.Security;
        MobilityRate = settings.MobilityRate;
        InfectionRate = settings.InfectionRate;
        Control = settings.Control;
        SpecialFlags = settings.SpecialFlags ?? new List<string>();

        // 初期状態
        Healthy = Population * 1000;
        Infected = 0;
        NearDeath = 0;
        Ghost = 0;
        Perished = 0;
        
        AreaStateCount = new AgentStateCount();
        
        InitializeCells(settings);
    }
    

    /// <summary>
    /// 初期化処理：セルを生成する
    /// </summary>
    private void InitializeCells(AreaSettingsSO settings)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        int cellPopulation = 100000; // 1セルあたりの人口
        int cellCount = Population / cellPopulation; // セルの個数計算
        
        // セルごとの市民と魔法士の人数を計算
        int cellCitizenPopulation = (int)(cellPopulation * (CitizenPopulation / (float)Population));
        int cellMagicSoldierPopulation = cellPopulation - cellCitizenPopulation;
        
        // セルを生成
        for (int i = 0; i < cellCount; i++)
        {
            _cells.Add(new MiniCell(i, cellCitizenPopulation, cellMagicSoldierPopulation, InfectionRate * 0.01f));
        }

        // あまりがあった場合
        int remainderPopulation = Population - (cellCount * cellPopulation);
        if (remainderPopulation > 0)
        {
            int remainderCitizenPopulation = (int)(remainderPopulation * (CitizenPopulation / (float)Population));
            int remainderMagicSoldierPopulation = remainderPopulation - remainderCitizenPopulation;
            
            _cells.Add(new MiniCell(cellCount, remainderCitizenPopulation, remainderMagicSoldierPopulation, InfectionRate * 0.01f));
        }
        
        Debug.Log($"{settings.Name.ToString()}エリアのセルの数：{_cells.Count} 実行時間: {stopwatch.ElapsedMilliseconds} ミリ秒");
    }

    /// <summary>
    /// 各セルに対してエージェントの情報を更新する指示を出す
    /// </summary>
    public async UniTask SimulateInfectionAsync()
    {
        List<Task> tasks = new List<Task>();
        foreach (var cell in _cells)
        {
            tasks.Add(Task.Run(() => cell.SimulateInfection()));
        }
        
        await Task.WhenAll(tasks);
        
        tasks.Add(Task.Run(UpdateStateCount));
        
        await Task.WhenAll(tasks);
    }
    
    /// <summary>
    /// AgentStateCountを更新する
    /// </summary>
    private void UpdateStateCount()
    {
        AreaStateCount.ResetStateCount();
        foreach (var cell in _cells)
        {
            AreaStateCount.UpdateStateCount(
                cell.StateCount.Healthy, cell.StateCount.Infected, cell.StateCount.NearDeath,
                cell.StateCount.Ghost, cell.StateCount.Perished, cell.StateCount.MagicSoldiers);
        }
    }
}
