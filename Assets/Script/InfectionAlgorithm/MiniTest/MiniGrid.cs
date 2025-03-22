using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

/// <summary>
/// 縮小テスト用のグリッドを管理するクラス
/// </summary>
public class MiniGrid
{
    private readonly MiniArea[,] _areas = new MiniArea[1 ,1]; // エリアデータの二次元配列
    private readonly AgentStateCount _totalStateCount; // ゲーム内に存在するエージェントの累計
    private readonly List<UniTask> _tasks = new List<UniTask>();
    
     public MiniGrid(List<AreaSettingsSO> areaSettings)
    {
        _totalStateCount = new AgentStateCount();
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
            if (x >= 0 && x < _areas.GetLength(0) && y >= 0 && y < _areas.GetLength(1))
            {
                // SOで設定した座標に基づいてエリアを配置
                _areas[x, y] = new MiniArea(areaSetting);
                Debug.Log($"エリア作成 ({x}, {y}) : {areaSetting.Name.ToString()}");
            }
            else
            {
                Debug.LogWarning($" MiniGrid：{areaSetting.Name}　({x}, {y}) は無効な座標です");
            }
        }
        Debug.Log($"グリッドの初期化完了");
    }
    
    /// <summary>
    /// 各エリアに対して更新処理を行う指示を出す
    /// </summary>
    public async UniTask SimulateInfectionAsync()
    {
        _tasks.Clear(); // 最初にTaskのリストをクリアして再利用
        
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
        
        UpdateStateCount();
    }

    /// <summary>
    /// 各エリアが保持しているTotalStateCountを集計して、自身のTotalStateCountを更新する
    /// 現在バッチ処理シングルスレッドバージョン
    /// </summary>
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
        }, "\ud83d\uddfa\ufe0fグリッド 全エージェントのステートの集計速度");
        
        // Gridの集計データをUIに反映
        Debug.Log($"[Grid 集計結果] 健常者: {_totalStateCount.Healthy} 感染者: {_totalStateCount.Infected} 仮死状態: {_totalStateCount.NearDeath} " +
                  $"亡霊: {_totalStateCount.Ghost} 完全死亡状態: {_totalStateCount.Perished}");
    }
}
