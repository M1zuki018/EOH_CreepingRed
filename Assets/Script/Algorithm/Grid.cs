using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 感染シミュレーション全体を管理するクラス
/// </summary>
public class Grid
{
    public Area[,] Areas { get; } = new Area[1 ,1]; // エリアを動的に変更する予定がないので二次元配列でやってみる
    public AgentStateCount TotalStateCount { get; private set; }

    public Grid(List<AreaSettingsSO> areaSettings)
    {
        TotalStateCount = new AgentStateCount();
        InitializeAreas(areaSettings);
    }

    /// <summary>
    /// 初期化処理：エリアを生成する
    /// </summary>
    private void InitializeAreas(List<AreaSettingsSO> areaSettings)
    {
        foreach (var areaSetting in areaSettings)
        {
            int x = areaSetting.X;
            int y = areaSetting.Y;
            
            // Areasの範囲を超えないかチェック
            if (x >= 0 && x < Areas.GetLength(0) && y >= 0 && y < Areas.GetLength(1))
            {
                // SOで設定した座標に基づいてエリアを配置
                Areas[x, y] = new Area(areaSetting);
                Debug.Log($"Area placed at ({x}, {y}) : {areaSetting.Name.ToString()}");
            }
            else
            {
                Debug.LogWarning($"Invalid area coordinates ({x}, {y}) for area: {areaSetting.Name}");
            }
        }
        Debug.Log($"Grid Initialize Finish");
    }
    
    /// <summary>
    /// 各エリアに対して更新処理を行う指示を出す
    /// </summary>
    public async UniTask SimulateInfectionAsync()
    {
        List<Task> tasks = new List<Task>();
        foreach (var area in Areas)
        {
            tasks.Add(Task.Run(() => area.SimulateInfectionAsync()));
        }
        
        await Task.WhenAll(tasks);  // すべてのタスクが完了するまで待機
        UpdateStateCount();
    }

    private void UpdateStateCount()
    {
        TotalStateCount.ResetStateCount(); // 一度リセットする
        
        foreach (var area in Areas)
        {
            TotalStateCount.UpdateStateCount(area.AreaStateCount.Healthy, area.AreaStateCount.Infected,
                area.AreaStateCount.NearDeath, area.AreaStateCount.Ghost, area.AreaStateCount.Perished,
                area.AreaStateCount.MagicSoldiers);
        }
        
        // Gridの集計データをUIに反映
        Debug.Log($"健常者: {TotalStateCount.Healthy}");
        Debug.Log($"感染者: {TotalStateCount.Infected}");
        Debug.Log($"仮死状態: {TotalStateCount.NearDeath}");
        Debug.Log($"亡霊: {TotalStateCount.Ghost}");
        Debug.Log($"完全死亡状態: {TotalStateCount.Perished}");
        Debug.Log($"魔法士: {TotalStateCount.MagicSoldiers}");
    }
    
    /// <summary>
    /// エリアデータを取得する（範囲外なら null）
    /// </summary>
    public Area? GetArea(int x, int y)
    {
        if (x < 0 || x >= Areas.GetLength(0) || y < 0 || y >= Areas.GetLength(1))
        {
            return null; // 範囲外なら null を返す
        }
        return Areas[x, y];
    }
}
