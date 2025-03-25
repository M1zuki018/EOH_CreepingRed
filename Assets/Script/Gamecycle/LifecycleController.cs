using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField, Comment("デバッグモード：本番環境にスキップしない")] private bool _debugMode = true;
    [SerializeField, Comment("Objectの生成ログを表示")] private bool _isLogFormatEnabled = true;
    [SerializeField, Comment("テストのログを表示")] private bool _isLoggingEnabled = true;
    
    [Header("設定")]
    [SerializeField] private List<GameObject> _prefabsToInstantiate = new List<GameObject>();
    private readonly List<ViewBase> _instantiatedViews = new List<ViewBase>();
    
    private async void Start()
    {
        if (!_debugMode && SceneManager.GetActiveScene().name != "Title")
        {
            Debug.Log($"\ud83d\udd34 本番環境: Titleシーンに強制遷移します");
            SceneManager.LoadScene("Title");
            return;
        }
        
        DebugLogHelper.IsObjectCreationLoggingEnabled = _isLogFormatEnabled;
        DebugLogHelper.IsTestLoggingEnabled = _isLoggingEnabled;
        
        await AutoInstantiate(); // インスタンス化
        
        await ExecuteLifecycleStep(view => view.OnAwake());
        await ExecuteLifecycleStep(view => view.OnUIInitialize());
        await ExecuteLifecycleStep(view => view.OnBind());
        await ExecuteLifecycleStep(view => view.OnStart());
        
        DebugLogHelper.LogImportant("\u2705 全てのオブジェクトの初期化が完了しました");
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
            Type viewType = GetViewBaseType(prefab);

            if (viewType == null)
            {
                Debug.LogWarning($"{prefab.name} のコンポーネントの型が特定できません。スキップします");
                return;
            }

            // 既存インスタンスを確認
            var existingInstance = FindObjectOfType(viewType) as ViewBase;
            if (existingInstance != null)
            {
                DebugLogHelper.LogObjectCreation("{0} の既存インスタンスが見つかったため、再生成しません", prefab.name);
                RegisterViewBase(existingInstance);
            }
            else // 既存インスタンスがなかったら作成する
            {
                CreateAndRegisterInstance(viewType, prefab);
            }
        }
        
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 新しいインスタンスを生成し登録
    /// GameObjectUtility.Instantiate<T>(プレハブ名)を自動実行する
    /// </summary>
    private void CreateAndRegisterInstance(Type viewType, GameObject prefab)
    {
        try
        {
            MethodInfo instantiateMethod =
                typeof(GameObjectUtility).GetMethod("Instantiate")?.MakeGenericMethod(viewType);

            if (instantiateMethod != null)
            {
                var newInstance = instantiateMethod.Invoke(null, new object[] { prefab }) as ViewBase;
                if (newInstance != null)
                {
                    RegisterViewBase(newInstance);
                }
            }

            Debug.Log($"{prefab.name} ({viewType.Name}) を自動生成しました！");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ {prefab.name}: 生成中にエラー発生 - {ex.Message}");
        }
        
    }

    /// <summary>
    /// プレハブから ViewBase を取得
    /// </summary>
    private Type GetViewBaseType(GameObject prefab)
    {
        return prefab.GetComponents<Component>().FirstOrDefault(c => c is ViewBase)?.GetType();
    }
    
    /// <summary>
    /// ViewBase を登録し、子オブジェクトに対しても検索を行う
    /// </summary>
    private void RegisterViewBase(ViewBase existingInstance)
    {
        _instantiatedViews.Add(existingInstance);
        foreach (Transform child in existingInstance.transform)
        {
            if (child.TryGetComponent(out ViewBase childViewBase))
            {
                RegisterViewBase(childViewBase); // 子オブジェクトがさらに子を持つ可能性があるので再帰的に呼び出す
            }
        }
    }
    
    /// <summary>
    /// 各ライフサイクルメソッドを全ビューに適用
    /// </summary>
    private async UniTask ExecuteLifecycleStep(Func<ViewBase, UniTask> lifecycleMethod)
    {
        await UniTask.WhenAll(_instantiatedViews.Select(lifecycleMethod));
    }
}