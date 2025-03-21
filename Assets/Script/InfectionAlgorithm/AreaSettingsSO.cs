using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリアの設定情報を保持するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "AreaSettings", menuName = "Create SO/AreaSettings")]
public class AreaSettingsSO : ScriptableObject
{
    // 基本情報
    public int X; // 座標
    public int Y;
    public SectionEnum Name; // 名称
    public AreaCategoryEnum Category; // 区域の区分
    public int Population; // 総人口（万人）
    public int CitizenPopulation; // 一般市民人口
    public int MagicSoldierPopulation; // 魔法士人口
    public float AreaSize; // 面積(㎢)
    public int PopulationDensity; // 人口密度(人/㎢)

    // 状態パラメータ
    public int Security; // 治安（0-100）
    public int MobilityRate; // 移動率（0-100）
    public int InfectionRate; // 感染成功率（%）
    public int Control; // 統制力（0-100）

    // 特殊フラグ（条件）
    public List<string> SpecialFlags; 
}
