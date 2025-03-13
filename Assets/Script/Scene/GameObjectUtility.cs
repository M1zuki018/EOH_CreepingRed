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
        
        return viewBase.GetComponent<T>();
    } 
}