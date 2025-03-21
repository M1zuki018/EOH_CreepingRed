using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Titleシーンスタートボーナス選択画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class StartBonusSelectionUIController : UIControllerBase
{
    [SerializeField, HighlightIfNull] private Button _tmp;
    
    public event Action OnSelect;
    
    protected override void RegisterEvents()
    {
        _tmp.onClick.AddListener(() => OnSelect?.Invoke());
    }

    protected override void UnregisterEvents()
    {
        _tmp.onClick.RemoveListener(() => OnSelect?.Invoke());
    }

    public override void Show() => CanvasVisibilityUtility.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityUtility.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityUtility.Block(_canvasGroup);
}
