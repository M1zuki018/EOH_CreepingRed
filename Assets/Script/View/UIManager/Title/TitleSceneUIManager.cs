using Cysharp.Threading.Tasks;
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
    
    public override UniTask OnAwake()
    {
        // Canvasコンポーネントの設定
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        
        return base.OnAwake();
    }

    public override UniTask OnBind()
    {
        _title.OnGameStart += () => TransitionView<IWindow>(_difficultySelection, _title);
        _title.OnGameSettings += () => OverlayView<IWindow>(_gameSettings, _title);
        
        _difficultySelection.OnSelect += () => TransitionView<IWindow>(_startBonusSelection, _difficultySelection);
        
        _startBonusSelection.OnSelect += () => TransitionView<IWindow>(_baseSelection, _startBonusSelection);
        
        _baseSelection.OnGameStart += TransitionScene;
        
        return base.OnBind();
    }

    public override UniTask OnStart()
    {
        _title.Show();
        _gameSettings.Hide();
        _difficultySelection.Hide();
        _startBonusSelection.Hide();
        _baseSelection.Hide();
        
        return base.OnStart();
    }
    
    /// <summary>
    /// シーン遷移
    /// </summary>
    private void TransitionScene()
    {
        TransitionScene("Dev_InGame");
    }
}
