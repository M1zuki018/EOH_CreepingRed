using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// シーン基盤
/// </summary>
public abstract class ViewBase : MonoBehaviour
{
    public virtual async UniTask OnAwake()
    {
        Debug.Log($"{gameObject.name} の Awake 実行");
        await UniTask.CompletedTask;
    }

    public virtual async UniTask OnUIInitialize()
    {
        Debug.Log($"{gameObject.name} の UI 初期化 実行");
        await UniTask.CompletedTask;
    }

    public virtual async UniTask OnStart()
    {
        Debug.Log($"{gameObject.name} の Start 実行");
        await UniTask.CompletedTask;
    }
}