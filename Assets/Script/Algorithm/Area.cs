using System;
using System.Collections.Generic;

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
    
    public List<Agent> Agents { get; } = new List<Agent>();
    public bool HasGhost { get; set; }  // 亡霊がいるか
    public double InfectionRisk { get; set; }  // 感染リスク

    /// <summary>
    /// エリアのコンストラクタ
    /// </summary>
    public Area(int x, int y, SectionEnum name, AreaCategoryEnum category, int population, int citizenPopulation, int magicSoldierPopulation, 
        float areaSize, int populationDensity, int security, int mobilityRate, 
        int infectionRate, int control, List<string> specialFlags = null)
    {
        X = x;
        Y = y;
        Name = name;
        Category = category;
        Population = population;
        CitizenPopulation = citizenPopulation;
        MagicSoldierPopulation = magicSoldierPopulation;
        AreaSize = areaSize;
        PopulationDensity = populationDensity;
        Security = security;
        MobilityRate = mobilityRate;
        InfectionRate = infectionRate;
        Control = control;
        SpecialFlags = specialFlags ?? new List<string>();

        // 初期状態
        Healthy = population * 10000;  // 10万人単位から実際の人数に換算
        Infected = 0;
        NearDeath = 0;
        Ghost = 0;
        Perished = 0;
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
