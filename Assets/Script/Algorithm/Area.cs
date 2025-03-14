using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
    
    private List<Cell> cells = new List<Cell>();
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
        CitizenPopulation = settings.citizenPopulation;
        MagicSoldierPopulation = settings.magicSoldierPopulation;
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
        
        // 人口10万人単位に分割してセルを生成
        int cellCount = Population / 100000;  // 10万人単位で分割
        for (int i = 0; i < cellCount; i++)
        {
            cells.Add(new Cell(i, 100000));
        }

        // あまりがあった場合
        if (Population - (cellCount * 100000) != 0)
        {
            cells.Add(new Cell(cellCount, Population - (cellCount * 100000)));
        }
        
        Debug.Log($"{settings.name.ToString()}エリアのセルの数：{cells.Count}");
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
