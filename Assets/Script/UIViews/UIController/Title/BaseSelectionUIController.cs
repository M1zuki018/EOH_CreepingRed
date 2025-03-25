using System;
using System.Collections.Generic;
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
    [SerializeField, HighlightIfNull] private List<Button> _gameStartButton;
    
    public event Action OnGameStart;

    protected override void RegisterEvents()
    {
        for (int i = 0; i < _gameStartButton.Count; i++)
        {
            var index = i;
            _gameStartButton[i].onClick.AddListener(() => HandleGameStart(index));
        }
    }

    protected override void UnregisterEvents()
    {
        for (int i = 0; i < _gameStartButton.Count; i++)
        {
            var index = i;
            _gameStartButton[i].onClick.RemoveListener(() => HandleGameStart(index));
        }
    }

    public override void Show() => CanvasVisibilityUtility.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityUtility.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityUtility.Block(_canvasGroup);

    /// <summary>
    /// ゲームスタートのイベントを発火すると同時に感染開始地底をGameSettingsに登録する
    /// </summary>
    private void HandleGameStart(int index)
    {
        OnGameStart?.Invoke();
        GameSettingsManager.StartPointIndex = index;
    }
}
