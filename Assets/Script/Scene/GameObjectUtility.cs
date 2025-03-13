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
        
        viewBase.OnAwake();
        viewBase.OnStart(); // 初期化処理を実行
        
        // 子オブジェクトをチェックして、ViewBase を継承しているものを探す
        foreach (Transform child in instance.transform)
        {
            var childViewBase = child.GetComponent<ViewBase>();
            if (childViewBase != null)
            {
                // 見つかった場合、OnAwake()とOnStart()を呼び出す
                childViewBase.OnAwake();
                childViewBase.OnStart();
            }
        }
        
        return viewBase.GetComponent<T>();
    } 
}