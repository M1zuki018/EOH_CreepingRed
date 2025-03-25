using UnityEngine;

/// <summary>
/// TitleシーンのUI全体を管理するManager
/// </summary>
public class TitleSceneUIManager : UIManagerBase
{
    [Header("UIControllerの登録")]
    [SerializeField, HighlightIfNull] private TitleUIController _title;
    [SerializeField, HighlightIfNull] private GameSettingsUIController _gameSettings;
    [SerializeField, HighlightIfNull] private DifficultySelectionUIController _difficultySelection;
    [SerializeField, HighlightIfNull] private StartBonusSelectionUIController _startBonusSelection;
    [SerializeField, HighlightIfNull] private BaseSelectionUIController _baseSelection;
    
    protected override void RegisterEvents()
    {
        // タイトル画面
        _title.OnGameStart += OnTitleGameStart;
        _title.OnGameSettings += OnTitleGameSettings;
        
        // 難易度選択画面
        _difficultySelection.OnSelect += OnDifficultySelect;
        
        // スタートボーナス選択画面
        _startBonusSelection.OnSelect += OnStartBonusSelect;
        
        // 拠点選択画面
        _baseSelection.OnGameStart += OnBaseSelectionGameStart;
    }

    protected override void InitializePanel()
    {
        _title.Show();
        _gameSettings.Hide();
        _difficultySelection.Hide();
        _startBonusSelection.Hide();
        _baseSelection.Hide();
    }
    
    /// <summary>
    /// シーン遷移
    /// </summary>
    private void TransitionScene()
    {
        TransitionScene("Dev_MiniLogicTest");
    }
    
    protected override void UnregisterEvents()
    {
        _title.OnGameStart -= OnTitleGameStart;
        _title.OnGameSettings -= OnTitleGameSettings;
        _difficultySelection.OnSelect -= OnDifficultySelect;
        _startBonusSelection.OnSelect -= OnStartBonusSelect;
        _baseSelection.OnGameStart -= OnBaseSelectionGameStart;
    }
    
    #region イベントハンドラー
    
    private void OnTitleGameStart() => TransitionView(_difficultySelection, _title);
    private void OnTitleGameSettings() => OverlayView(_gameSettings, _title);
    private void OnDifficultySelect() => TransitionView(_startBonusSelection, _difficultySelection);
    private void OnStartBonusSelect() => TransitionView(_baseSelection, _startBonusSelection);
    private void OnBaseSelectionGameStart() => TransitionScene();
    
    #endregion
}
