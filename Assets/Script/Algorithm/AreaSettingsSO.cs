using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリアの設定情報を保持するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "AreaSettings", menuName = "Create SO/AreaSettings")]
public class AreaSettingsSO : ScriptableObject
{
    // 基本情報
    public int x; // 座標
    public int y;
    public SectionEnum name; // 名称
    public AreaCategoryEnum category; // 区域の区分
    public int population; // 総人口（万人）
    public int citizenPopulation; // 一般市民人口
    public int magicSoldierPopulation; // 魔法士人口
    public float areaSize; // 面積(㎢)
    public int populationDensity; // 人口密度(人/㎢)

    // 状態パラメータ
    public int security; // 治安（0-100）
    public int mobilityRate; // 移動率（0-100）
    public int infectionRate; // 感染成功率（%）
    public int control; // 統制力（0-100）

    // 特殊フラグ（条件）
    public List<string> specialFlags; 
}
