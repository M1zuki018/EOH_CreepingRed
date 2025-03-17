using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// InGameシーンのUI全体を管理するManager
/// </summary>
public class InGameSceneUIManager : ViewBase
{
    [SerializeField, HighlightIfNull] private BaseViewUIController _baseView;
    
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
        _baseView.OnMacroView += () => ShowMacroView();
        
        return base.OnBind();
    }

    private void ShowMacroView()
    {
        
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
        SceneManager.LoadScene("Dev_Result");
    }

    public override UniTask OnStart()
    {
        _baseView.Show();
        
        return base.OnStart();
    }
}
