using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 独自のライフサイクルを定義した基底クラス
/// </summary>
public abstract class ViewBase : MonoBehaviour
{
    /// <summary>
    /// 他クラスに干渉しない処理
    /// </summary>
    public virtual UniTask OnAwake()
    {
        DebugLogHelper.LogObjectCreation("[ViewBase] {0} の Awake 実行", gameObject.name);
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// UI表示の初期化
    /// </summary>
    public virtual UniTask OnUIInitialize()
    {
        DebugLogHelper.LogObjectCreation("[ViewBase] {0} の UIInitialize 実行", gameObject.name);
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// event Actionの登録など他のクラスと干渉する処理
    /// </summary>
    public virtual UniTask OnBind()
    {
        DebugLogHelper.LogObjectCreation("[ViewBase] {0} の Bind 実行", gameObject.name);
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// ゲーム開始前最後に実行される処理
    /// </summary>
    public virtual UniTask OnStart()
    {
        DebugLogHelper.LogObjectCreation("[ViewBase] {0} の Start 実行", gameObject.name);
        return UniTask.CompletedTask;
    }
}