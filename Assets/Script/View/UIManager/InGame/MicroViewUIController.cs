using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    [SerializeField, HighlightIfNull] private Button _closeButton;
    [SerializeField, HighlightIfNull] private Button _nextButton;
    [SerializeField, HighlightIfNull] private Button _backButton;
    
    [Header("MicroViewの設定")] 
    [SerializeField, HighlightIfNull] private Text _nameText;
    [SerializeField, HighlightIfNull] private Text _explainText;
    
    private List<AreaViewSettingsSO> _areaSettings;
    private int _selectedArea; // 現在表示中のエリアのIndex
    private CanvasGroup _canvasGroup;
    public event Action OnMacroView;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    
        _closeButton.onClick.AddListener(() => OnMacroView?.Invoke()); // 閉じるボタンを押したら全体ビューへ
        _nextButton.onClick.AddListener(() => ChangeArea(1).Forget());
        _backButton.onClick.AddListener(() => ChangeArea(-1).Forget());
        
        return base.OnUIInitialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize(List<AreaViewSettingsSO> areaSettings)
    {
        _areaSettings = areaSettings; // areaSettingsリストを受け取り
    }
    
    /// <summary>
    /// 指定エリアのビューを開く
    /// </summary>
    public void ShowMicroView(int index)
    {
        _selectedArea = index;
        
        _nameText.text = StateExtensions.ToJapanese(_areaSettings[index].Name); // エリア名
        _explainText.text = _areaSettings[index].Explaination; // エリアの説明
        
        // アニメーション
        _nameText.DOFade(1, 0.5f);
        _explainText.DOFade(1, 0.5f);
        
        Show();
    }

    /// <summary>
    /// 前後のエリアに移動する
    /// </summary>
    private async UniTask ChangeArea(int operation)
    {
        float fadeDuration = 0.5f;
        
        // フェードアウト処理
        _nameText.DOFade(0, fadeDuration);
        _explainText.DOFade(0, fadeDuration);
        
        await UniTask.WaitForSeconds(fadeDuration);
        
        _selectedArea = (_selectedArea + operation) % 19; // 19エリアで循環するようにする
        if (_selectedArea < 0) _selectedArea += 19; // 負のインデックスを防ぐ
        ShowMicroView(_selectedArea);
    }
    
    public void Show()
    {
        CanvasVisibilityController.Show(_canvasGroup);
    }
    
    public void Hide()
    {
        CanvasVisibilityController.Hide(_canvasGroup);
        _nameText.DOFade(0, 0.5f);
        _explainText.DOFade(0, 0.5f);
    }
    
    public void Block()
    {
        CanvasVisibilityController.Block(_canvasGroup);
    }
}
