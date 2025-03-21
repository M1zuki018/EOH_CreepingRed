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
public class MicroViewUIController : UIControllerBase
{
    [Header("UIパネルとしての設定")]
    [SerializeField, HighlightIfNull] private Button _closeButton;
    [SerializeField, HighlightIfNull] private Button _nextButton;
    [SerializeField, HighlightIfNull] private Button _backButton;
    
    [Header("MicroViewの設定")] 
    [SerializeField, HighlightIfNull] private Image _backgroundImage;
    [SerializeField, HighlightIfNull] private Text _nameText;
    [SerializeField, HighlightIfNull] private Text _explainText;

    [Header("開発中オンリー")] [SerializeField] private Text _day;
    
    private List<AreaViewSettingsSO> _areaSettings; // エリアデータの参照
    private int _selectedArea; // 現在表示中のエリアのIndex
    public event Action OnMacroView;

    protected override void RegisterEvents()
    {
        _closeButton.onClick.AddListener(() => OnMacroView?.Invoke()); // 閉じるボタンを押したら全体ビューへ
        _nextButton.onClick.AddListener(() => ChangeArea(1).Forget());
        _backButton.onClick.AddListener(() => ChangeArea(-1).Forget());
    }

    protected override void UnregisterEvents()
    {
        _closeButton.onClick.RemoveListener(() => OnMacroView?.Invoke());
        _nextButton.onClick.RemoveListener(() => ChangeArea(1).Forget());
        _backButton.onClick.RemoveListener(() => ChangeArea(-1).Forget());
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
        
        var area = _areaSettings[index];
        
        _nameText.text = ExtensionsUtility.SectionEnumToJapanese(area.Name); // エリア名
        _explainText.text = area.Explaination; // エリアの説明
        
        if (area.Background != null)
        {
            _backgroundImage.sprite = area.Background; // 背景変更
            Dev(area.Background); // TODO: あとで消す
        }
        
        // アニメーション
        _nameText.DOFade(1, 0.5f);
        _explainText.DOFade(1, 0.5f);
        
        Show();
    }
    
    /// <summary>
    /// 開発中の悪ノリ処理
    /// </summary>
    private void Dev(Sprite sprite)
    {
        _day.gameObject.SetActive(sprite.name == "tmp");
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
    
    public override void Show() => CanvasVisibilityUtility.Show(_canvasGroup);
    
    public override void Hide()
    {
        CanvasVisibilityUtility.Hide(_canvasGroup);
        _nameText.DOFade(0, 0.5f);
        _explainText.DOFade(0, 0.5f);
    }
    
    public override void Block() => CanvasVisibilityUtility.Block(_canvasGroup);
}
