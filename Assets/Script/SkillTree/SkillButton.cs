using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルボタン用のクラス
/// (表示非表示を切り替える仕組みを作るとなったらUIControllerBase継承に変更してもいいかも)
/// </summary>
public class SkillButton　: ViewBase
{
    /// <summary>
    /// スキルデータ
    /// </summary>
    [SerializeField, ExpandableSO] private SkillDataSO _skillData;
    public SkillDataSO SkillData => _skillData;
    
    /// <summary>
    /// 解放済みかどうかのフラグ
    /// </summary>
    private bool _isUnlocked = false;
    public bool IsUnlocked => _isUnlocked;
    
    private Button _button; // 自身のボタンコンポーネント
    private Color _defaultColor; // ボタンの初期色
    
    public event Action<SkillButton> OnClick; 

    public override UniTask OnUIInitialize()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick); // クリックイベントを登録
        InitializeButtonUI();

        return base.OnUIInitialize();
    }

    /// <summary>
    /// UIの初期化
    /// </summary>
    private void InitializeButtonUI()
    {
        _defaultColor = _button.image.color; // 初期色を保存
        _button.image.color = _defaultColor * 0.6f; // 色を少し暗めに変更
    }

    /// <summary>
    /// スキルをアンロックする
    /// </summary>
    public void Unlock()
    {
        _isUnlocked = true;
        _button.image.color = _defaultColor;
    }
    
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }
    
    private void OnButtonClick() => OnClick?.Invoke(this);
    
#if UNITY_EDITOR
    
    /// <summary>
    /// 外部からスキルデータをセットする
    /// </summary>
    public void SetSkillData(SkillDataSO skillData)
    {
        _skillData = skillData;
    }

    /// <summary>
    /// スキルデータがセットされているか確認する
    /// </summary>
    public bool SkillDataCheck() => _skillData== null;
    
#endif
}
