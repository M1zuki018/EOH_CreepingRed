using UnityEngine;

/// <summary>
/// CanvasGroupの表示/非表示を制御する静的クラス
/// </summary>
public static class CanvasVisibilityUtility
{
    /// <summary>
    /// 表示する
    /// </summary>
    public static void Show(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// 非表示にする
    /// </summary>
    public static void Hide(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 反応しないようにする（オーバーレイ表示の背面となる画面向け）
    /// </summary>
    public static void Block(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
