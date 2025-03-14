using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

/// <summary>
/// 各区域を管理するクラス
/// </summary>
public class Area
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
    
    private List<Cell> _cells = new List<Cell>();
    public double InfectionRisk { get; set; }  // 感染リスク

    /// <summary>
    /// エリアのコンストラクタ
    /// </summary>
    public Area(AreaSettingsSO settings)
    {
        X = settings.x;
        Y = settings.y;
        Name = settings.name;
        Category = settings.category;
        Population = settings.population * 10000;
        CitizenPopulation = settings.citizenPopulation * 10000;
        MagicSoldierPopulation = settings.magicSoldierPopulation * 10000;
        AreaSize = settings.areaSize;
        PopulationDensity = settings.populationDensity;
        Security = settings.security;
        MobilityRate = settings.mobilityRate;
        InfectionRate = settings.infectionRate;
        Control = settings.control;
        SpecialFlags = settings.specialFlags ?? new List<string>();

        // 初期状態
        Healthy = Population * 10000;  // 10万人単位から実際の人数に換算
        Infected = 0;
        NearDeath = 0;
        Ghost = 0;
        Perished = 0;
        
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
            _cells.Add(new Cell(i, cellCitizenPopulation, cellMagicSoldierPopulation));
        }

        // あまりがあった場合
        int remainderPopulation = Population - (cellCount * cellPopulation);
        if (remainderPopulation > 0)
        {
            int remainderCitizenPopulation = (int)(remainderPopulation * (CitizenPopulation / (float)Population));
            int remainderMagicSoldierPopulation = remainderPopulation - remainderCitizenPopulation;
            
            _cells.Add(new Cell(cellCount, remainderCitizenPopulation, remainderMagicSoldierPopulation));
        }
        
        Debug.Log($"{settings.name.ToString()}エリアのセルの数：{_cells.Count} 実行時間: {stopwatch.ElapsedMilliseconds} ミリ秒");
    }

    /// <summary>
    /// 各セルに対してエージェントの情報を更新する指示を出す
    /// </summary>
    public async UniTask SimulateInfectionAsync()
    {
        List<Task> tasks = new List<Task>();
        foreach (var cell in _cells)
        {
            tasks.Add(Task.Run(() => cell.SimulateInfection(5f, InfectionRate)));
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 感染状況を更新する
    /// </summary>
    public void UpdateEnvironment(int newInfected, int newGhost, int newDeaths)
    {
        Infected += newInfected;
        Ghost += newGhost;
        Perished += newDeaths;
        Healthy -= (newInfected + newDeaths);

        // データ整合性チェック
        if (Healthy < 0) Healthy = 0;
        if (Infected < 0) Infected = 0;
        if (Ghost < 0) Ghost = 0;
        if (Perished < 0) Perished = 0;
    }
    
    /// <summary>
    /// 治安・統制力を変化させる
    /// </summary>
    public void ModifySecurity(int securityChange, int controlChange)
    {
        Security = Math.Clamp(Security + securityChange, 0, 100);
        Control = Math.Clamp(Control + controlChange, 0, 100);
    }

    /// <summary>
    /// 特殊フラグを追加
    /// </summary>
    public void AddSpecialFlag(string flag)
    {
        if (!SpecialFlags.Contains(flag))
        {
            SpecialFlags.Add(flag);
        }
    }

    /// <summary>
    /// 特殊フラグを削除
    /// </summary>
    public void RemoveSpecialFlag(string flag)
    {
        SpecialFlags.Remove(flag);
    }
}
