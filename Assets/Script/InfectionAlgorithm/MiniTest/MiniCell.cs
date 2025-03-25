using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class MiniCell
{
    private readonly int _id; // セル自体のID
    private readonly MiniAgentManager _agentManager; // シミュレーションを行うクラス
    private readonly AgentStateCount _cellStateCount;
    public AgentStateCount CellStateCount => _cellStateCount; // エージェントのカウント用のクラス
    public bool IsActive { get; private set; } // シミュレーションが起動中かどうか
    public bool Spreading { get; private set; } // 他のセルに感染を広げるかどうか

    private JobHandle _jobHandle; // エージェント生成JobのHandle
    
    public MiniCell(int id, int citizen, float regionMod, bool infection)
    {
        _id = id;
        _cellStateCount = new AgentStateCount();
        _agentManager = new MiniAgentManager(regionMod);
        StopwatchHelper.Measure(() => InitializeAgents(citizen, infection).Forget(),"Agent生成完了");
    }

    /// <summary>
    /// エージェントの生成（非同期でセットアップ）
    /// </summary>
    private async UniTask InitializeAgents(int citizen, bool infection)
    {
        await _agentManager.InitializeAgents(citizen);
        if(infection) _agentManager.Infection(1); // 感染させるセルに対してのみフラグを立てる
    }

    /// <summary>
    /// 感染開始処理
    /// </summary>
    public void Infection(int count)
    {
        _agentManager.Infection(count);
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public async UniTask SimulateInfection()
    {
        StopwatchHelper.Measure(() =>
            {
                if (IsActive)
                {
                    _jobHandle = _agentManager.SimulateInfection(); // Jobを設定
                    _jobHandle.Complete();
                }

            },$"\ud83d\udfe6セル(ID:{_id}) 感染シミュレーションの更新速度");
        await UpdateStateCount();
    }

    /// <summary>
    /// Quadtree内のエージェントのステートを集計する
    /// </summary>
    private async UniTask UpdateStateCount()
    {
        _cellStateCount.ResetStateCount();

        await StopwatchHelper.MeasureAsync(async () =>
            {
                var allAgents = _agentManager.GetAllAgents(); // AgentManagerから全てのエージェントを取得する
                
                foreach (var agent in allAgents)
                {
                    _cellStateCount.AddState(agent.State); // 各ステートをカウント
                }

                // セル内の感染率が8割を越えたらフラグを立てる
                if ((float)_cellStateCount.Infected / allAgents.Count() > 0.8f)
                {
                    Spreading = true;
                }
                
                // 全員死亡 もしくは 全員健康状態のときは処理をスキップするようにする
                if (_cellStateCount.NearDeath == allAgents.Count() || _cellStateCount.Healthy == 0)
                {
                    IsActive = false;
                    return;
                }

                if (!IsActive)
                {
                    IsActive = true;
                }

            }, $"\ud83d\udfe6セル(ID:{_id}) Quadtreeのステート集計速度");
    }
}
