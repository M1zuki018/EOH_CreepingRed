using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン基盤
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _debugMode = true;
    
    [Header("設定")]
    [SerializeField] private List<GameObject> _prefabsToInstantiate = new List<GameObject>();
    private List<ViewBase> _instantiatedViews = new List<ViewBase>();
    
    private async void Start()
    {
        if (_debugMode)
        {
            Debug.Log($"デバッグモード：現状のシーンを維持します");
        }
        else
        {
            // 本番モードの遷移処理
            if (SceneManager.GetActiveScene().name != "Title")
            {
                Debug.Log($"本番モード：本番用Titleシーンに遷移します");
                SceneManager.LoadScene("Title");
                return;
            }
        }
        
        await AutoInstantiate();
        
        await ExecuteAwake();
        await ExecuteUIInitialize();
        await ExecuteStart();
    }

    /// <summary>
    /// 登録されたプレハブを順番にインスタンス化する
    /// </summary>
    private async UniTask AutoInstantiate()
    {
        foreach (var prefab in _prefabsToInstantiate)
        {
            if (prefab == null) continue;

            // 推測されるコンポーネントの型を取得
            Type inferredType = FindViewBaseType(prefab);

            if (inferredType == null)
            {
                Debug.LogWarning($"{prefab.name} のコンポーネントの型が特定できません。スキップします");
                return;
            }

            // 既存インスタンスを確認
            var existingInstance = FindObjectOfType(inferredType) as ViewBase;

            if (existingInstance != null)
            {
                Debug.Log($"{prefab.name} の既存インスタンスが見つかったため、再生成しません");
                _instantiatedViews.Add(existingInstance);
                foreach (Transform child in existingInstance.transform)
                {
                    var childViewBase = child.GetComponent<ViewBase>();
                    if (childViewBase != null)
                    {
                        _instantiatedViews.Add(childViewBase);
                    }
                }
            }
            else
            {
                // `GameObjectUtility.Instantiate<T>(プレハブ名)` を自動実行
                MethodInfo instantiateMethod = typeof(GameObjectUtility).GetMethod("Instantiate")
                    .MakeGenericMethod(inferredType);
                var newInstance = instantiateMethod.Invoke(null, new object[] { prefab }) as ViewBase;
                
                if (newInstance != null)
                {
                    _instantiatedViews.Add(newInstance);
                    
                    foreach (Transform child in newInstance.transform)
                    {
                        var childViewBase = child.GetComponent<ViewBase>();
                        if (childViewBase != null)
                        {
                            Debug.Log("取得");
                            _instantiatedViews.Add(childViewBase);
                        }
                    }
                }

                Debug.Log($"{prefab.name} ({inferredType.Name}) を自動生成しました！");
            }
        }
        
        await UniTask.CompletedTask;
    }
    
    // プレハブの最初の `ViewBase` 派生コンポーネントの型を取得
    private Type FindViewBaseType(GameObject prefab)
    {
        Component[] components = prefab.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is ViewBase)
            {
                return component.GetType();
            }
        }
        return null;
    }
    
    private async UniTask ExecuteAwake()
    {
        List<UniTask> awakeTasks = new List<UniTask>();
        foreach (var view in _instantiatedViews)
        {
            awakeTasks.Add(view.OnAwake()); 
        }
        await UniTask.WhenAll(awakeTasks);
    }

    private async UniTask ExecuteUIInitialize()
    {
        List<UniTask> awakeTasks = new List<UniTask>();
        foreach (var view in _instantiatedViews)
        {
            awakeTasks.Add(view.OnUIInitialize());
        }
        await UniTask.WhenAll(awakeTasks);
    }

    private async UniTask ExecuteStart()
    {
        List<UniTask> awakeTasks = new List<UniTask>();
        foreach (var view in _instantiatedViews)
        {
            awakeTasks.Add(view.OnStart());
        }
        await UniTask.WhenAll(awakeTasks);
    }
}