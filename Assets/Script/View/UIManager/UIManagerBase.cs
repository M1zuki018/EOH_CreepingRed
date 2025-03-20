using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// UIManagerのベースクラス
/// </summary>
public abstract class UIManagerBase : ViewBase
{
    public override UniTask OnAwake()
    {
        SetCanvas();
        return base.OnAwake();
    }
    
    /// <summary>
    /// Canvasの設定を行う
    /// </summary>
    private void SetCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
        }
        else
        {
            Debug.LogError($"{nameof(UIManagerBase)} Canvasが見つかりませんでした");
        }
    }
    
    /// <summary>
    /// 画面遷移
    /// </summary>
    protected void TransitionView(IWindow show, IWindow hide)
    {
        show.Show();
        hide.Hide();
    }

    /// <summary>
    /// オーバーレイとしての画面表示
    /// </summary>
    protected void OverlayView(IWindow show, IWindow block)
    {
        show.Show();
        block.Block();
    }
    
    /// <summary>
    /// シーン遷移
    /// </summary>
    protected void TransitionScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
