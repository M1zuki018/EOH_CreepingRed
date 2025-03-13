using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Titleシーン難易度選択画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DifficultySelectionUIController : ViewBase, IWindow
{
    [SerializeField, HighlightIfNull] private Button _breeze;
    [SerializeField, HighlightIfNull] private Button _storm;
    [SerializeField, HighlightIfNull] private Button _catastrophe;
    [SerializeField, HighlightIfNull] private Button _unknown;
    [SerializeField, HighlightIfNull] private Button _custom;
    
    private CanvasGroup _canvasGroup;
    public event Action OnSelect;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    
        _breeze.onClick.AddListener(() => Registration(0));
        _storm.onClick.AddListener(() => Registration(1));
        _catastrophe.onClick.AddListener(() => Registration(2));
        _unknown.onClick.AddListener(() => Registration(3));
        _custom.onClick.AddListener(() => Registration(4));
        
        return base.OnUIInitialize();
    }

    private void Registration(int index)
    {
        GameSettingsManager.Difficulty = index; // 難易度をセット
        OnSelect?.Invoke();
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
