using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Titleシーン拠点選択画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class BaseSelectionUIController : UIControllerBase
{
    [SerializeField, HighlightIfNull] private Button _gameStartButton;
    
    public event Action OnGameStart;

    protected override void RegisterEvents()
    {
        _gameStartButton.onClick.AddListener(() => OnGameStart?.Invoke());
    }

    protected override void UnregisterEvents()
    {
        _gameStartButton.onClick.RemoveListener(() => OnGameStart?.Invoke());
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
