using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーン拠点画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class BaseViewUIController : ViewBase, IWindow
{
    [SerializeField, HighlightIfNull] private Button _macroViewButton;
    [SerializeField, HighlightIfNull] private Image _ezechielImage;
    
    private CanvasGroup _canvasGroup;
    public event Action OnMacroView;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    
        _macroViewButton.onClick.AddListener(() => OnMacroView?.Invoke());
        return base.OnUIInitialize();
    }
    
    public void Show()
    {
        CanvasVisibilityController.Show(_canvasGroup);
    }
    
    public void Hide()
    {
        CanvasVisibilityController.Hide(_canvasGroup);
    }
    
    public void Block()
    {
        CanvasVisibilityController.Block(_canvasGroup);
    }
}
