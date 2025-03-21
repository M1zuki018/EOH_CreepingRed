using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーンエゼキエルのスキルツリー画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class EzechielSkillTreeUIController : UIControllerBase
{
    [Header("InGameViewとしてのセットアップ")]
    [SerializeField, HighlightIfNull] private Button _closeButton;
    
    [Header("スキルツリーのためのセットアップ")]
    [SerializeField, HighlightIfNull, Comment("リタのスキルツリーボタン")] private Button _ritaTreeButton;
    
    public event Action OnClose;
    public event Action OnShowRitaTree;

    protected override void UnregisterEvents()
    {
        _closeButton.onClick.AddListener(() => OnClose?.Invoke());
        _ritaTreeButton.onClick.AddListener(() => OnShowRitaTree?.Invoke());
    }
    
    protected override void RegisterEvents()
    {
        _closeButton.onClick.RemoveListener(() => OnClose?.Invoke());
        _ritaTreeButton.onClick.RemoveListener(() => OnShowRitaTree?.Invoke());
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
