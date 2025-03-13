using UnityEngine;

/// <summary>
/// TitleシーンのUI全体を管理するManager
/// </summary>
public class TitleSceneUIManager : ViewBase
{
    [SerializeField] private GameObject[] _windows = new GameObject[3];
    private IWindow[] _windowsUI = new IWindow[3];
    
    public override void OnAwake()
    {
        // Canvasコンポーネントの設定
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        for(int i = 0; i < 3; i++)
        {
            _windowsUI[i] = _windows[i].GetComponent<IWindow>();
        }
    }

    public override void OnStart()
    {
        //_windowsUI[0].Show();
    }
}
