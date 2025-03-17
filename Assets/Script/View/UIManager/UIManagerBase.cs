using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

/// <summary>
/// UIManagerのベースクラス
/// </summary>
public abstract class UIManagerBase : ViewBase
{
    public override UniTask OnAwake() { return base.OnAwake(); }
    
    public override UniTask OnBind() { return base.OnBind(); }
    
    public override UniTask OnStart() { return base.OnStart(); }
    
    /// <summary>
    /// 画面遷移
    /// </summary>
    protected void TransitionView<T>(T show, T hide) where T : IWindow
    {
        show.Show();
        hide.Hide();
    }

    /// <summary>
    /// オーバーレイとしての画面表示
    /// </summary>
    protected void OverlayView<T>(T show, T block) where T : IWindow
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
