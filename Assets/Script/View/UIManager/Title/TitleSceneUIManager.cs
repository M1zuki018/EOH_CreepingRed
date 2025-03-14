using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TitleシーンのUI全体を管理するManager
/// </summary>
public class TitleSceneUIManager : ViewBase
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

    /// <summary>
    /// 画面遷移
    /// </summary>
    private void TransitionView<T>(T show, T hide) where T : IWindow
    {
        show.Show();
        hide.Hide();
    }

    /// <summary>
    /// オーバーレイとしての画面表示
    /// </summary>
    private void OverlayView<T>(T show, T block) where T : IWindow
    {
        show.Show();
        block.Block();
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    private void TransitionScene()
    {
        SceneManager.LoadScene("Dev_InGame");
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
}
