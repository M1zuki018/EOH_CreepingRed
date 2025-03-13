using UnityEngine;

/// <summary>
/// CanvasGroupの表示/非表示を制御するクラス
/// </summary>
public class CanvasVisibilityController
{
    /// <summary>
    /// 表示する
    /// </summary>
    public void Show(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// 非表示にする
    /// </summary>
    public void Hide(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
