using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Titleシーンタイトル画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class TitleUIController : UIControllerBase
{
    [SerializeField, HighlightIfNull] private Button _gameStartButton;
    [SerializeField, HighlightIfNull] private Button _gameSettingsButton;
    
    public event Action OnGameStart; // 準備画面に遷移するイベント
    public event Action OnGameSettings; // 設定画面に遷移するイベント

    protected override void RegisterEvents()
    {
        _gameStartButton.onClick.AddListener(() => OnGameStart?.Invoke());
        _gameSettingsButton.onClick.AddListener(() => OnGameSettings?.Invoke());
    }

    protected override void UnregisterEvents()
    {
        _gameStartButton.onClick.RemoveListener(() => OnGameStart?.Invoke());
        _gameSettingsButton.onClick.RemoveListener(() => OnGameSettings?.Invoke());
    }

    public override void Show() => CanvasVisibilityUtility.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityUtility.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityUtility.Block(_canvasGroup);
}
