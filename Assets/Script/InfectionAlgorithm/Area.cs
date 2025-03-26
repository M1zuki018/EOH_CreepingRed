using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 各区域を管理するクラス
/// </summary>
public class Area
{
    // 基本情報
    private readonly SectionEnum _section; // 名前
    private readonly int _citizenPopulation; // 一般市民人口
    private readonly int _infectionRate; // 感染成功率（%）
    private List<string> _specialFlags; // 特殊フラグ（条件）
    
    private List<Cell> _cells = new List<Cell>(); // セルのリスト
    private int _infectionIndex = 0; // 感染が行われているセルのインデックス
    private readonly AgentStateCount _areaStateCount;
    public AgentStateCount AreaStateCount => _areaStateCount; // Areaクラスのエージェントの状態の集計結果
    public event Action StateUpdated; // 健康, 感染, 仮死の数値を通知するイベント
    
    private List<UniTask> _tasks = new List<UniTask>(); // シミュレーション更新タスクのリスト

    /// <summary>
    /// エリアのコンストラクタ
    /// </summary>
    public Area(AreaSettingsSO settings, bool infection)
    {
        _section = settings.Name;
        _citizenPopulation = settings.CitizenPopulation * 1000;
        _infectionRate = settings.InfectionRate;
        _specialFlags = settings.SpecialFlags ?? new List<string>();
        _areaStateCount = new AgentStateCount();
        
        AreaStateCountRegister.Instance.RegisterArea(_section, _areaStateCount, this); // 辞書に登録
        InitializeCells(settings, infection);
    }

    /// <summary>
    /// 初期化処理：セルを生成する
    /// </summary>
    private void InitializeCells(AreaSettingsSO settings, bool infection)
    {
        StopwatchHelper.Measure(() =>
        {
            int cellPopulation = 100000; // 1セルあたりの人口
            int cellCount = _citizenPopulation / cellPopulation; // セルの個数計算
            _cells = new List<Cell>(cellCount + 1); // 事前にリストを確保
                
            // セルを生成
            for (int i = 0; i < cellCount; i++)
            {
                // 最初のセルだけ感染させる
                if (infection)
                {
                    _cells.Add(new Cell(i, cellPopulation, _infectionRate * 0.01f, i == 0));
                }
                else
                {
                    _cells.Add(new Cell(i, cellPopulation, _infectionRate * 0.01f, false));
                }
            }

            // あまりがあった場合
            int remainderPopulation = _citizenPopulation - (cellCount * cellPopulation);
            if (remainderPopulation > 0)
            {
                // 他にセルが登録されていなければあまりのセルを感染させる
                if(infection)_cells.Add(new Cell(cellCount, remainderPopulation, _infectionRate * 0.01f, cellCount == 0));
                else _cells.Add(new Cell(cellCount, remainderPopulation, _infectionRate * 0.01f, false));
            }
                
            DebugLogHelper.LogTestOnly($"{settings.Name.ToString()}エリアのセルの数：{_cells.Count}");
        }, "\ud83c\udfde\ufe0fエリア　セル生成時間");
    }

    /// <summary>
    /// 各セルに対してエージェントの情報を更新する指示を出す
    /// </summary>
    public async UniTask SimulateInfectionAsync()
    {
        _tasks.Clear(); // リセット
        _tasks = new List<UniTask>(_cells.Count); // セルの数だけUniTaskのリストを事前に確保しておく
        
        for (int i = 0; i < _cells.Count; i++)
        {
            _tasks.Add(_cells[i].SimulateInfection()); // アクティブなセルに対してのみ更新を行う
        }

        await UniTask.WhenAll(_tasks); // 全てのセルのシミュレーション更新を待機
        
        await UpdateStateCount();
    }

    /// <summary>
    /// AgentStateCountを更新する
    /// </summary>
    private async UniTask UpdateStateCount()
    {
        _areaStateCount.ResetStateCount(); // 一旦リセット

        await StopwatchHelper.MeasureAsync(async () =>
        {
            int totalHealthy = 0, totalInfected = 0, totalNearDeath = 0;
            int totalGhost = 0, totalPerished = 0;
            
            // 並列処理でエリアの状態を集計
            Parallel.ForEach(_cells, (cell) =>
            {
                Interlocked.Add(ref totalHealthy, cell.CellStateCount.Healthy);
                Interlocked.Add(ref totalInfected, cell.CellStateCount.Infected);
                Interlocked.Add(ref totalNearDeath, cell.CellStateCount.NearDeath);
                Interlocked.Add(ref totalGhost, cell.CellStateCount.Ghost);
                Interlocked.Add(ref totalPerished, cell.CellStateCount.Perished);
            });
            
            // 集計結果を反映する
            _areaStateCount.UpdateStateCount(
                totalHealthy, totalInfected, totalNearDeath,
                totalGhost, totalPerished
            );
            
            // 集計結果を通知（イベント発火）
            StateUpdated?.Invoke();
            
        }, "\ud83c\udfde\ufe0fエリア 各セルのステートの集計速度");
        
        // 感染フラグが立っていたら、次のセルに感染を広める
        if (_infectionIndex < _cells.Count && _cells[_infectionIndex].Spreading)
        {
            if (_infectionIndex < _cells.Count - 1) // 範囲を越えないようにする
            {
                _cells[++_infectionIndex].Infection(1);
            }
        }
    }
}
