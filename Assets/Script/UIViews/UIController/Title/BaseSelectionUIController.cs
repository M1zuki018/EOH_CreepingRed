using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField, HighlightIfNull] private List<Button> _baseSelectButton;
    public event Action OnGameStart;
    
    protected override void RegisterEvents()
    {
        _gameStartButton.onClick.AddListener(HandleGameStart);
        for (int i = 0; i < _baseSelectButton.Count; i++)
        {
            var index = i;
            _baseSelectButton[i].onClick.AddListener(() => HandleSelectBase(index));
        }
        
        _gameStartButton.interactable = false; // 拠点が選択されるまではインタラクティブ出来ないようにする
    }

    protected override void UnregisterEvents()
    {
        _gameStartButton.onClick.RemoveListener(HandleGameStart);
        for (int i = 0; i < _baseSelectButton.Count; i++)
        {
            var index = i;
            _baseSelectButton[i].onClick.RemoveListener(() => HandleSelectBase(index));
        }
    }

    public override void Show() => CanvasVisibilityUtility.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityUtility.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityUtility.Block(_canvasGroup);

    /// <summary>
    /// ゲームスタートのイベントを発火すると同時に感染開始地底をGameSettingsに登録する
    /// </summary>
    private void HandleSelectBase(int index)
    {
        GameSettingsManager.StartPointIndex = index;
        _gameStartButton.interactable = true;
    }
    private void HandleGameStart() => OnGameStart?.Invoke();
}
