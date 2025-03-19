using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InGameシーンスキルツリー画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class SkillTreeUIController : ViewBase, IWindow
{
    [Header("InGameViewとしてのセットアップ")]
    [SerializeField, HighlightIfNull] private Button _closeButton;
    
    [Header("スキルツリーのためのセットアップ")]
    [SerializeField] private List<SkillBase> _skillTrees = new List<SkillBase>();
    [SerializeField, HighlightIfNull, Comment("スキル名のエリア")] private Text _skillName;
    [SerializeField, HighlightIfNull, Comment("スキル説明のエリア")] private Text _skillDescription;
    [SerializeField, HighlightIfNull, Comment("解放コストのエリア")] private Text _point;
    [SerializeField, HighlightIfNull, Comment("解放ボタン")] private Button _unlockButton;
    [SerializeField, HighlightIfNull, Comment("エゼキエルのスキルツリーボタン")] private Button _ezechielButton;
    
    private CanvasGroup _canvasGroup;
    public event Action OnClose;
    public event Action OnShowEzechielTree;
    public event Action OnUnlock;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _closeButton.onClick.AddListener(() => OnClose?.Invoke());
        _ezechielButton.onClick.AddListener(() => OnShowEzechielTree?.Invoke());

        foreach (var skillTree in _skillTrees)
        {
            skillTree.SetUIController(this);
        }
        
        SkillTextsUpdate(" ", " ", " ");
        ChangeUnlockButton(false);
        _unlockButton.onClick.AddListener(UnlockedSkill);
        
        return base.OnUIInitialize();
    }

    /// <summary>
    /// スキル表示のUIを更新する
    /// </summary>
    public void SkillTextsUpdate(string name, string description, string point)
    {
        _skillName.text = name;
        _skillDescription.text = description;
        _point.text = point;
    }

    /// <summary>
    /// スキルの解放ボタンにインタラクティブできるかどうかを切り替える
    /// </summary>
    public void ChangeUnlockButton(bool isUnlock)
    {
        _unlockButton.interactable = isUnlock;
    }

    /// <summary>
    /// スキルを解放する
    /// </summary>
    public void UnlockedSkill()
    {
        OnUnlock?.Invoke();
        Debug.Log("スキル解放");
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

    private void OnDestroy()
    {
        // 登録解除
        _unlockButton.onClick.RemoveAllListeners(); 
        _closeButton.onClick.RemoveAllListeners();
        _ezechielButton.onClick.RemoveAllListeners();
    }
}
