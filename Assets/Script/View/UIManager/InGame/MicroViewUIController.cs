using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーン各エリアの画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MicroViewUIController : ViewBase, IWindow
{
    [Header("UIパネルとしての設定")]
    [SerializeField, HighlightIfNull] private Button _backButton;

    [Header("MicroViewの設定")] 
    [SerializeField, HighlightIfNull] private Text _nameText;
    [SerializeField, HighlightIfNull] private Text _explainText;
    
    private AreaSettingsSO _selectedArea; // 現在表示中のエリアの参照
    private CanvasGroup _canvasGroup;
    public event Action OnMacroView;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    
        _backButton.onClick.AddListener(() => OnMacroView?.Invoke());
        return base.OnUIInitialize();
    }

    /// <summary>
    /// 指定エリアのビューを開く
    /// </summary>
    public void ShowMicroView(AreaSettingsSO areaSettings)
    {
        _selectedArea = areaSettings;
        _nameText.text = StateExtensions.ToJapanese(areaSettings.Name);
        Show();
    }
    
    public void Show()
    {
        CanvasVisibilityController.Show(_canvasGroup);
    }
    
    public void Hide()
    {
        CanvasVisibilityController.Hide(_canvasGroup);
    }
    
    public void Block()
    {
        CanvasVisibilityController.Block(_canvasGroup);
    }
}
