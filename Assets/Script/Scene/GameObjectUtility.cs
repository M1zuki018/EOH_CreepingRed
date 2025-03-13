using UnityEngine;

/// <summary>
/// GameObjectに関するUtilityクラス
/// </summary>
public static class GameObjectUtility
{
    /// <summary>
    /// プレハブをインスタンス化し、ViewBase の派生クラスを返す
    /// </summary>
    public static T Instantiate<T>(GameObject prefab) where T : ViewBase
    {
        if (prefab == null)
        {
            Debug.LogError("Prefabがnullです");
            return null;
        }
        
        var instance= Object.Instantiate(prefab); // インスタンス生成
        var viewBase = instance.GetComponent<T>(); // T型のコンポーネント取得
        
        if (viewBase == null)
        {
            Debug.LogError($"Prefabに {typeof(T).Name} のコンポーネントがアタッチされていません");
            return null;
        }
        
        InitializeViewBase(viewBase);
        
        return viewBase.GetComponent<T>();
    } 
    
    /// <summary>
    /// 初期化処理
    /// </summary>
    public static void InitializeViewBase(ViewBase viewBase)
    {
        MethodHandle(viewBase);
        
        foreach (Transform child in viewBase.transform)
        {
            var childViewBase = child.GetComponent<ViewBase>();
            if (childViewBase != null)
            {
                MethodHandle(childViewBase);
            }
        }
    }

    /// <summary>
    /// ViewBaseのMethodを呼び出す
    /// </summary>
    private static void MethodHandle(ViewBase viewBase)
    {
        viewBase.OnAwake();
        viewBase.OnUIInitialize();
        viewBase.OnStart();
    }
}