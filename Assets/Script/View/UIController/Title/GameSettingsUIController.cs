using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Titleシーン設定画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class GameSettingsUIController : UIControllerBase
{
    //[SerializeField, HighlightIfNull] private Button _gameStartButton;
    
    //public event Action OnGameStart;

    protected override void RegisterEvents()
    {
        // イベント登録
    }

    protected override void UnregisterEvents()
    {
        // イベント解除
    }

    public override void Show() => CanvasVisibilityController.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityController.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityController.Block(_canvasGroup);
}
