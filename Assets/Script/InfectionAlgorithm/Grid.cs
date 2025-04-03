using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 感染シミュレーション全体を管理するクラス
/// </summary>
public class Grid
{
    private readonly Area[,] _areas = new Area[5 ,4]; // エリアクラスの二次元配列
    public Area[,] Areas => _areas;
    private readonly AgentStateCount _totalStateCount; // ゲーム内に存在するエージェントの累計
    private readonly List<UniTask> _tasks = new List<UniTask>();
    public static event Action<int, int, int> StateUpdated; // 健康, 感染, 仮死の数値を通知するイベント

    public Grid(List<AreaSettingsSO> areaSettings)
    {
        _totalStateCount = new AgentStateCount();
        InitializeAreas(areaSettings);
    }

    /// <summary>
    /// 初期化処理：エリアを生成する
    /// </summary>
    private void InitializeAreas(List<AreaSettingsSO> areaSettings)
    {
        StopwatchHelper.AlwaysUse(() =>
        {
            int index = GameSettingsManager.StartPointIndex;
            int startX = index % 5;
            int startY = index / 5;
            
            foreach (var areaSetting in areaSettings)
            {
                int x = areaSetting.X;
                int y = areaSetting.Y;

                // Areasの範囲を超えないかチェック
                if (x >= 0 && x < _areas.GetLength(0) && y >= 0 && y < _areas.GetLength(1))
                {
                    var check = startX == x && startY == y;
                    // SOで設定した座標に基づいてエリアを配置
                    _areas[x, y] = new Area(areaSetting, check);
                    DebugLogHelper.LogTestOnly($"エリア作成 ({x}, {y}) : {areaSetting.Name.ToString()}");
                }
                else
                {
                    Debug.LogWarning($" MiniGrid：{areaSetting.Name}　({x}, {y}) は無効な座標です");
                }
            }
        },"\ud83d\uddfa\ufe0fグリッド 初期化");
    }
    
    /// <summary>
    /// 各エリアに対して更新処理を行う指示を出す
    /// </summary>
    public async UniTask SimulateInfectionAsync()
    {
        _tasks.Clear(); // 最初にTaskのリストをクリアして再利用
        
        await StopwatchHelper.AlwaysUseAsync(async () =>
        {
            for (int x = 0; x < _areas.GetLength(0); x++)
            {
                for (int y = 0; y < _areas.GetLength(1); y++)
                {
                    var area = _areas[x, y];
                    if (area != null)
                    {
                        _tasks.Add(area.SimulateInfectionAsync());
                    }
                }
            }
        
            await UniTask.WhenAll(_tasks);  // すべてのタスクが完了するまで待機
        }, "\ud83d\uddfa\ufe0fグリッド シミュレーション更新");
        
        
        UpdateStateCount();
    }

    private void UpdateStateCount()
    {
        _totalStateCount.ResetStateCount(); // 一度リセットする
        
        StopwatchHelper.Measure(() =>
        {
            // バッチ処理で追加
            int totalHealthy = 0, totalInfected = 0, totalNearDeath = 0;
            int totalGhost = 0, totalPerished = 0, totalMagicSoldiers = 0;

            for (int x = 0; x < _areas.GetLength(0); x++)
            {
                for (int y = 0; y < _areas.GetLength(1); y++)
                {
                    var area = _areas[x, y];
                    if (area == null) continue;

                    totalHealthy += area.AreaStateCount.Healthy;
                    totalInfected += area.AreaStateCount.Infected;
                    totalNearDeath += area.AreaStateCount.NearDeath;
                    totalGhost += area.AreaStateCount.Ghost;
                    totalPerished += area.AreaStateCount.Perished;
                }
            }

            _totalStateCount.UpdateStateCount(
                totalHealthy, totalInfected, totalNearDeath, totalGhost, totalPerished);
            
            // 集計結果を通知（イベント発火）
            StateUpdated?.Invoke(totalHealthy, totalInfected, totalNearDeath);
            
        }, "\ud83d\uddfa\ufe0fグリッド 全エージェントのステートの集計速度");
        
        // Gridの集計データをUIに反映
        Debug.Log($"[Grid 集計結果] 健常者: {_totalStateCount.Healthy} 感染者: {_totalStateCount.Infected} 仮死状態: {_totalStateCount.NearDeath} " +
                  $"亡霊: {_totalStateCount.Ghost} 完全死亡状態: {_totalStateCount.Perished}");
    }
}
