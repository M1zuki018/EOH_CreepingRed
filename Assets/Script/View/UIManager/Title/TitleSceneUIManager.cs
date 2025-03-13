using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// TitleシーンのUI全体を管理するManager
/// </summary>
public class TitleSceneUIManager : ViewBase
{
    [SerializeField, HighlightIfNull] private TitleUIController _title;
    [SerializeField, HighlightIfNull] private GameSettingsUIController _gameSettings;
    [SerializeField, HighlightIfNull] private StandbyUIController _standby;
    
    public override UniTask OnAwake()
    {
        // Canvasコンポーネントの設定
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        
        Debug.Log(_title.name);
        Debug.Log(_gameSettings.name);
        Debug.Log(_standby.name);
        
        return base.OnAwake();
    }

    public override UniTask OnBind()
    {
        _title.OnGameStart += () => TransitionView<IWindow>(_standby, _title);
        _title.OnGameSettings += () => OverlayView<IWindow>(_gameSettings, _title);
        
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

    private void OverlayView<T>(T show, T block) where T : IWindow
    {
        show.Show();
        block.Block();
    }

    public override UniTask OnStart()
    {
        _title.Show();
        return base.OnStart();
    }
}
