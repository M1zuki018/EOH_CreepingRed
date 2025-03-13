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
public class TitleUIController : ViewBase
{
    [SerializeField, HighlightIfNull] private Button _gameStartButton;
    [SerializeField, HighlightIfNull] private Button _gameSettingsButton;

    public event Action OnGameStart; // 準備画面に遷移するイベント
    public event Action OnGameSettings; // 設定画面に遷移するイベント

    public override void OnUIInitialize()
    {
        _gameStartButton.onClick.AddListener(() => OnGameStart?.Invoke());
        _gameSettingsButton.onClick.AddListener(() => OnGameSettings?.Invoke());
    }
}
