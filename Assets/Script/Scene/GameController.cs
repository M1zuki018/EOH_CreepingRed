using System;
using System.Collections.Generic;
using System.Reflection;
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
    
    private void Start()
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
        
        AutoInstantiate();
    }

    /// <summary>
    /// 登録されたプレハブを順番にインスタンス化する
    /// </summary>
    private void AutoInstantiate()
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
                GameObjectUtility.InitializeViewBase(existingInstance); // 初期化処理
            }
            else
            {
                // `GameObjectUtility.Instantiate<T>(プレハブ名)` を自動実行
                MethodInfo instantiateMethod = typeof(GameObjectUtility).GetMethod("Instantiate")
                    .MakeGenericMethod(inferredType);
                instantiateMethod.Invoke(null, new object[] { prefab });

                Debug.Log($"{prefab.name} ({inferredType.Name}) を自動生成しました！");
            }
        }
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
}