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

    private JobHandle _jobHandle; // エージェント生成JobのHandle
    
    public MiniCell(int id, int citizen, float regionMod)
    {
        _id = id;
        _cellStateCount = new AgentStateCount();
        _agentManager = new MiniAgentManager(regionMod);
        StopwatchHelper.Measure(() => InitializeAgents(citizen).Forget(),"Agent生成完了");
    }

    /// <summary>
    /// エージェントの生成（非同期でセットアップ）
    /// </summary>
    private async UniTask InitializeAgents(int citizen)
    {
        await _agentManager.InitializeAgents(citizen);
        _agentManager.Infection(1);
    }

    /// <summary>
    /// 感染シミュレーション
    /// </summary>
    public async UniTask SimulateInfection()
    {
        StopwatchHelper.Measure(() =>
        {
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

        await StopwatchHelper.MeasureAsync(async () => 
            {
                var allAgents = _agentManager.GetAllAgents(); // AgentManagerから全てのエージェントを取得する

                foreach (var agent in allAgents)
                {
                    _cellStateCount.AddState(agent.State); // 各ステートをカウント
                }

            }, $"\ud83d\udfe6セル(ID:{_id}) Quadtreeのステート集計速度");
    }
}
