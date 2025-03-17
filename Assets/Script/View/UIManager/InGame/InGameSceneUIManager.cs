using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーンのUI全体を管理するManager
/// </summary>
public class InGameSceneUIManager : UIManagerBase
{
    [SerializeField, HighlightIfNull] private Text _timeText;
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
        new TimeView(_timeText, FindAnyObjectByType<GameManager>().TimeManager);
        _baseView.OnMacroView += () => ShowMacroView();
        
        return base.OnBind();
    }

    private void ShowMacroView()
    {
        
    }

    public override UniTask OnStart()
    {
        _baseView.Show();
        
        return base.OnStart();
    }
    
    /// <summary>
    /// シーン遷移
    /// </summary>
    private void TransitionScene()
    {
        TransitionScene("Dev_Result");
    }
}
