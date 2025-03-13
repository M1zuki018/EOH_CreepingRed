using System;
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
    public virtual async UniTask OnAwake()
    {
        Debug.Log($"{gameObject.name} の Awake 実行");
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// UI表示の初期化
    /// </summary>
    public virtual async UniTask OnUIInitialize()
    {
        Debug.Log($"{gameObject.name} の UI 初期化 実行");
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// event Actionの登録など他のクラスと干渉する処理
    /// </summary>
    public virtual async UniTask OnBind()
    {
        Debug.Log($"{gameObject.name} の Bind 実行");
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// ゲーム開始前最後に実行される処理
    /// </summary>
    public virtual async UniTask OnStart()
    {
        Debug.Log($"{gameObject.name} の Start 実行");
        await UniTask.CompletedTask;
    }
}