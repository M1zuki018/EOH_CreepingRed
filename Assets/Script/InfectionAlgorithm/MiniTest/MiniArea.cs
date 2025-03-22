using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

/// <summary>
/// 縮小テスト用各区域を管理するクラス
/// </summary>
public class MiniArea
{
    #region パラメータ

    // 基本情報
    private readonly int _citizenPopulation; // 一般市民人口
    private readonly int _infectionRate; // 感染成功率（%）

    // 特殊フラグ（条件）
    private List<string> _specialFlags;

    #endregion
    
    private List<MiniCell> _cells = new List<MiniCell>();
    private readonly AgentStateCount _areaStateCount;
    public AgentStateCount AreaStateCount => _areaStateCount;
    
    private List<UniTask> _tasks = new List<UniTask>();

    /// <summary>
    /// エリアのコンストラクタ
    /// </summary>
    public MiniArea(AreaSettingsSO settings)
    {
        _citizenPopulation = settings.CitizenPopulation * 1000;
        _infectionRate = settings.InfectionRate;
        _specialFlags = settings.SpecialFlags ?? new List<string>();
        
        _areaStateCount = new AgentStateCount();
        InitializeCells(settings);
    }

    /// <summary>
    /// 初期化処理：セルを生成する
    /// </summary>
    private void InitializeCells(AreaSettingsSO settings)
    {
        StopwatchHelper.Measure(() =>
            {
                int cellPopulation = 100000; // 1セルあたりの人口
                int cellCount = _citizenPopulation / cellPopulation; // セルの個数計算
                
                _cells = new List<MiniCell>(cellCount + 1);
                
                // セルを生成
                for (int i = 0; i < cellCount; i++)
                {
                    _cells.Add(new MiniCell(i, cellPopulation, _infectionRate * 0.01f));
                }

                // あまりがあった場合
                int remainderPopulation = _citizenPopulation - (cellCount * cellPopulation);
                if (remainderPopulation > 0)
                {
                    _cells.Add(new MiniCell(cellCount, remainderPopulation, _infectionRate * 0.01f));
                }
                
                DebugLogHelper.LogImportant($"{settings.Name.ToString()}エリアのセルの数：{_cells.Count}");
            }, "\ud83c\udfde\ufe0fエリア　セル生成時間");
        
        _tasks = new List<UniTask>(_cells.Count); // セルの数だけUniTaskのリストを事前に確保しておく
    }

    /// <summary>
    /// 各セルに対してエージェントの情報を更新する指示を出す
    /// </summary>
    public async UniTask SimulateInfectionAsync()
    {
        _tasks.Clear(); // リセット

        for (int i = 0; i < _cells.Count; i++)
        {
            _tasks.Add(_cells[i].SimulateInfection());
        }

        await UniTask.WhenAll(_tasks);
        
        await UpdateStateCount();
    }

    /// <summary>
    /// AgentStateCountを更新する
    /// </summary>
    private async UniTask UpdateStateCount()
    {
        _areaStateCount.ResetStateCount(); // 一旦リセット

        StopwatchHelper.Measure(() =>
        {
            int totalHealthy = 0, totalInfected = 0, totalNearDeath = 0;
            int totalGhost = 0, totalPerished = 0;
            
            // 並列処理でエリアの状態を集計
            Parallel.ForEach(_cells, (cell) =>
            {
                Interlocked.Add(ref totalHealthy, cell.StateCount.Healthy);
                Interlocked.Add(ref totalInfected, cell.StateCount.Infected);
                Interlocked.Add(ref totalNearDeath, cell.StateCount.NearDeath);
                Interlocked.Add(ref totalGhost, cell.StateCount.Ghost);
                Interlocked.Add(ref totalPerished, cell.StateCount.Perished);
            });
            
            _areaStateCount.UpdateStateCount(
                totalHealthy, totalInfected, totalNearDeath,
                totalGhost, totalPerished
            );
        }, "\ud83c\udfde\ufe0fエリア 各セルのステートの集計速度");

        await UniTask.CompletedTask; // 非同期完了を通知
    }
}
