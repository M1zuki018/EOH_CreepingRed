using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーン都市全体画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MacroViewUIController : ViewBase, IWindow
{
    [SerializeField, HighlightIfNull] private Button _skillTreeButton;
    [SerializeField, HighlightIfNull] private Button _closeButton;
    [SerializeField, HighlightIfNull] private Button[] _areaButton = new Button[19];
    
    private CanvasGroup _canvasGroup;
    private List<AreaViewSettingsSO> _areaSettings;
    public event Action OnSkillTree;
    public event Action<int> OnArea;
    public event Action OnClose;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    
        _skillTreeButton.onClick.AddListener(() => OnSkillTree?.Invoke());
        _closeButton.onClick.AddListener(() => OnClose?.Invoke());

        for (int i = 0; i < _areaButton.Length; i++)
        {
            var index = i;
            _areaButton[i].onClick.AddListener(() => OnArea?.Invoke(index));
        }
        
        return base.OnUIInitialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize(List<AreaViewSettingsSO> areaSettings)
    {
        _areaSettings = areaSettings; // areaSettingsリストを受け取り
        SetAreaButtonText();
    }

    /// <summary>
    /// ボタンのテキストを変更する
    /// </summary>
    private void SetAreaButtonText()
    {
        for (int i = 0; i < _areaSettings.Count; i++)
        {
            Text text = _areaButton[i].GetComponentInChildren<Text>();
            text.text = _areaSettings[i].Name.ToString();
        }
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
