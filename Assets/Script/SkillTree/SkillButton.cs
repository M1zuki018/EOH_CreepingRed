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
    private Button _button;

    public override UniTask OnBind()
    {
        _button = GetComponent<Button>();
        return base.OnBind();
    }

    /// <summary>
    /// 外部からスキルデータをセットする
    /// </summary>
    public void SetSkillData(SkillDataSO skillData)
    {
        _skillData = skillData;
    }
}
