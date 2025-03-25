using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Jobs;

/// <summary>
/// 縮小テスト用エージェント約10万人が詰められたセル。感染シミュレーションを行う部分
/// </summary>
public class MiniCell
{
    private readonly int _id; // セル自体のID
    private readonly MiniAgentManager _agentManager; // シミュレーションを行うクラス
    private readonly AgentStateCount _cellStateCount;
    public AgentStateCount CellStateCount => _cellStateCount; // エージェントのカウント用のクラス
    private bool _isActive; // シミュレーションが起動中かどうか
    public bool Spreading { get; private set; } // 他のセルに感染を広げるかどうか

    private JobHandle _jobHandle; // エージェント生成JobのHandle
    
    public MiniCell(int id, int citizen, float regionMod)
    {
        _id = id;
        _cellStateCount = new AgentStateCount();
        _agentManager = new MiniAgentManager(regionMod);
        StopwatchHelper.TestOnlyMeasure(() => InitializeAgents(citizen).Forget(),"Agent生成完了");
    }

    /// <summary>
    /// エージェントの生成（非同期でセットアップ）
    /// </summary>
    private async UniTask InitializeAgents(int citizen)
    {
        await _agentManager.InitializeAgents(citizen);
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
        StopwatchHelper.TestOnlyMeasure(() =>
            {
                if (!_isActive) return;

                _jobHandle = _agentManager.SimulateInfection(); // Jobを設定
                _jobHandle.Complete();

            },$"\ud83d\udfe6セル(ID:{_id}) 感染シミュレーションの更新速度");
        await UpdateStateCount();
    }

    /// <summary>
    /// Quadtree内のエージェントのステートを集計する
    /// </summary>
    private async UniTask UpdateStateCount()
    {
        _cellStateCount.ResetStateCount();

        await StopwatchHelper.TestOnlyMeasureAsync(async () =>
            {
                var allAgents = _agentManager.GetAllAgents(); // AgentManagerから全てのエージェントを取得する
                int agentsCount = allAgents.Count();
                
                foreach (var agent in allAgents)
                {
                    _cellStateCount.AddState(agent.State); // 各ステートをカウント
                }
                
                HandleInfectionSpread(agentsCount);
                HandleCellActivation(agentsCount);
                
            }, $"\ud83d\udfe6セル(ID:{_id}) Cellのステート集計速度");
    }

    /// <summary>
    /// セル内の感染率が8割を越えたらフラグを立てる
    /// </summary>
    private void HandleInfectionSpread(int allAgents)
    {
        if ((float)_cellStateCount.Infected / allAgents > 0.8f)
        {
            Spreading = true;
        }
    }

    /// <summary>
    /// セルのアクティブ状態を更新する
    /// </summary>
    private void HandleCellActivation(int allAgents)
    {
        // 全員死亡 もしくは 全員健康状態のときは処理をスキップするようにする
        if (_cellStateCount.NearDeath == allAgents || _cellStateCount.Healthy == 0)
        {
            _isActive = false;
            return;
        }

        // 一つ目の条件を抜けた場合で、まだ起動していなかったら起動する
        if (!_isActive)
        {
            _isActive = true;
        }
    }
}
