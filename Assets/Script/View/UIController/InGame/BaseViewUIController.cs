using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーン拠点画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class BaseViewUIController : UIControllerBase
{
    [SerializeField, HighlightIfNull] private Button _macroViewButton;
    [SerializeField, HighlightIfNull] private Image _ezechielImage;
    
    public event Action OnMacroView;

    protected override void RegisterEvents()
    {
        _macroViewButton.onClick.AddListener(() => OnMacroView?.Invoke());
    }

    protected override void UnregisterEvents()
    {
        _macroViewButton.onClick.RemoveListener(() => OnMacroView?.Invoke());
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
