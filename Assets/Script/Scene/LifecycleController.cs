using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン基盤
/// </summary>
public class LifecycleController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _debugMode = true;
    
    [Header("設定")]
    [SerializeField] private List<GameObject> _prefabsToInstantiate = new List<GameObject>();
    private List<ViewBase> _instantiatedViews = new List<ViewBase>();
    
    private async void Start()
    {
        if (!_debugMode && SceneManager.GetActiveScene().name != "Title")
        {
            Debug.Log($"本番モード：本番用Titleシーンに遷移します");
            SceneManager.LoadScene("Title");
            return;
        }
        
        await AutoInstantiate(); // インスタンス化
        
        await ExecuteLifecycleStep(view => view.OnAwake());
        await ExecuteLifecycleStep(view => view.OnUIInitialize());
        await ExecuteLifecycleStep(view => view.OnStart());
        
        DebugLogHelper.LogImportant("生成終了");
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
                FindChildViewBase(existingInstance);
            }
            else // 既存インスタンスがなかったら作成する
            {
                // `GameObjectUtility.Instantiate<T>(プレハブ名)` を自動実行
                MethodInfo instantiateMethod = typeof(GameObjectUtility).GetMethod("Instantiate")
                    ?.MakeGenericMethod(inferredType);
                
                if (instantiateMethod != null)
                {
                    var newInstance = instantiateMethod.Invoke(null, new object[] { prefab }) as ViewBase;
                
                    if (newInstance != null)
                    {
                        _instantiatedViews.Add(newInstance);
                        FindChildViewBase(newInstance);
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
    
    /// <summary>
    /// 生成したインスタンスの子オブジェクトのViewBaseを取得する
    /// </summary>
    private void FindChildViewBase(ViewBase existingInstance)
    {
        foreach (Transform child in existingInstance.transform)
        {
            var childViewBase = child.GetComponent<ViewBase>();
            if (childViewBase != null)
            {
                _instantiatedViews.Add(childViewBase);
            }
        }
    }
    
    /// <summary>
    /// 取得したviewに対して、Awake、Startなどのそれぞれの処理を実行する
    /// </summary>
    private async UniTask ExecuteLifecycleStep(Func<ViewBase, UniTask> lifecycleMethod)
    {
        await UniTask.WhenAll(_instantiatedViews.Select(lifecycleMethod));
    }
}