using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルボタン用のクラス
/// </summary>
public class SkillButton　: ViewBase
{
    [SerializeField, Expandable] private SkillDataSO _skillData;
    public SkillDataSO SkillData => _skillData;
    private bool _isUnlocked = false; // 解放済みか
    public bool IsUnlocked => _isUnlocked;
    private Button _button;
    
    public event Action<SkillButton> OnClick; 

    public override UniTask OnUIInitialize()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnClick?.Invoke(this));
        
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
    }
    
    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners(); // 購読解除
    }
}
