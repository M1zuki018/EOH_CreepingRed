using UnityEngine;

/// <summary>
/// TitleシーンのUI全体を管理するManager
/// </summary>
public class TitleSceneUIManager : UIManagerBase
{
    [SerializeField, HighlightIfNull] private TitleUIController _title;
    [SerializeField, HighlightIfNull] private GameSettingsUIController _gameSettings;
    [SerializeField, HighlightIfNull] private DifficultySelectionUIController _difficultySelection;
    [SerializeField, HighlightIfNull] private StartBonusSelectionUIController _startBonusSelection;
    [SerializeField, HighlightIfNull] private BaseSelectionUIController _baseSelection;
    
    protected override void RegisterEvents()
    {
        // タイトル画面
        _title.OnGameStart += () => TransitionView(_difficultySelection, _title);
        _title.OnGameSettings += () => OverlayView(_gameSettings, _title);
        
        // 難易度選択画面
        _difficultySelection.OnSelect += () => TransitionView(_startBonusSelection, _difficultySelection);
        
        // スタートボーナス選択画面
        _startBonusSelection.OnSelect += () => TransitionView(_baseSelection, _startBonusSelection);
        
        // 拠点選択画面
        _baseSelection.OnGameStart += TransitionScene;
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
        TransitionScene("Dev_InGame");
    }
}
