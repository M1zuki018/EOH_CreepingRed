using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UIControllerのベースクラス
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIControllerBase : ViewBase, IWindow
{
    protected CanvasGroup _canvasGroup;

    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        RegisterEvents();
        return base.OnUIInitialize();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    /// <summary>
    /// イベント登録を行う（各UIControllerで実装する）
    /// </summary>
    protected abstract void RegisterEvents();
    
    /// <summary>
    /// イベント解除を行う（各UIControllerで実装する）
    /// </summary>
    protected abstract void UnregisterEvents();
    
    public virtual void Show() { }
    public virtual void Hide() { }
    public virtual void Block() { }
}
