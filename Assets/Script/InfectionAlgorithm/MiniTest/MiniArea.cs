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
    private readonly int _population;// 総人口（万人）
    private readonly int _citizenPopulation; // 一般市民人口
    private readonly int _magicSoldierPopulation; // 魔法士人口
    private readonly int _infectionRate; // 感染成功率（%）

    // 特殊フラグ（条件）
    public List<string> SpecialFlags { get; } 

    #endregion
    
    private readonly List<MiniCell> _cells = new List<MiniCell>();
    private readonly AgentStateCount _areaStateCount;
    public AgentStateCount AreaStateCount => _areaStateCount;
    
    private List<UniTask> _tasks = new List<UniTask>();

    /// <summary>
    /// エリアのコンストラクタ
    /// </summary>
    public MiniArea(AreaSettingsSO settings)
    {
        _population = settings.Population * 1000;
        _citizenPopulation = settings.CitizenPopulation * 1000;
        _magicSoldierPopulation = settings.MagicSoldierPopulation * 1000;
        _infectionRate = settings.InfectionRate;
        SpecialFlags = settings.SpecialFlags ?? new List<string>();
        
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
                int cellCount = _population / cellPopulation; // セルの個数計算

                // セルごとの市民と魔法士の人数を計算
                int cellCitizenPopulation = (int)(cellPopulation * (_citizenPopulation / (float)_population));
                int cellMagicSoldierPopulation = cellPopulation - cellCitizenPopulation;

                // セルを生成
                for (int i = 0; i < cellCount; i++)
                {
                    _cells.Add(new MiniCell(i, cellCitizenPopulation, cellMagicSoldierPopulation, _infectionRate * 0.01f));
                }

                // あまりがあった場合
                int remainderPopulation = _population - (cellCount * cellPopulation);
                if (remainderPopulation > 0)
                {
                    int remainderCitizenPopulation = (int)(remainderPopulation * (_citizenPopulation / (float)_population));
                    int remainderMagicSoldierPopulation = remainderPopulation - remainderCitizenPopulation;

                    _cells.Add(new MiniCell(cellCount, remainderCitizenPopulation, remainderMagicSoldierPopulation, _infectionRate * 0.01f));
                }
            }, $"{settings.Name.ToString()}エリアのセルの数：{_cells.Count}");
        
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
            int totalGhost = 0, totalPerished = 0, totalMagicSoldiers = 0;
            
            // 並列処理でエリアの状態を集計
            Parallel.ForEach(_cells, (cell) =>
            {
                Interlocked.Add(ref totalHealthy, cell.StateCount.Healthy);
                Interlocked.Add(ref totalInfected, cell.StateCount.Infected);
                Interlocked.Add(ref totalNearDeath, cell.StateCount.NearDeath);
                Interlocked.Add(ref totalGhost, cell.StateCount.Ghost);
                Interlocked.Add(ref totalPerished, cell.StateCount.Perished);
                Interlocked.Add(ref totalMagicSoldiers, cell.StateCount.MagicSoldiers);
            });
            
            _areaStateCount.UpdateStateCount(
                totalHealthy, totalInfected, totalNearDeath,
                totalGhost, totalPerished, totalMagicSoldiers
            );
        }, "エリアのカウント集計速度");

        await UniTask.CompletedTask; // 非同期完了を通知
    }
}
