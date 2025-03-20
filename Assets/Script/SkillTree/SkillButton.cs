using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルボタン用のクラス
/// </summary>
public class SkillButton　: ViewBase
{
    /// <summary>
    /// スキルデータ
    /// </summary>
    [SerializeField, Expandable] private SkillDataSO _skillData;
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
        _button.onClick.AddListener(() => OnClick?.Invoke(this)); // クリックイベントを登録
        
        // ボタンUIの調整
        _defaultColor = _button.image.color; // 初期色を保存
        _button.image.color = new Color(_defaultColor.r - 0.4f, _defaultColor.g - 0.4f, _defaultColor.b - 0.4f); // 色を少し暗めに変更
        
        return base.OnUIInitialize();
    }

    /// <summary>
    /// 外部からスキルデータをセットする
    /// </summary>
    public void SetSkillData(SkillDataSO skillData)
    {
        _skillData = skillData;
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
        _button.onClick.RemoveAllListeners(); // 購読解除
    }
}
