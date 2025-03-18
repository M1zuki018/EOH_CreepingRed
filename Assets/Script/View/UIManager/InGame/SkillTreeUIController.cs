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
    [Header("スキルツリーのためのセットアップ")]
    [SerializeField] private List<SkillBase> _skillTrees = new List<SkillBase>();
    [SerializeField, HighlightIfNull, Comment("スキル名のエリア")] private Text _skillName;
    [SerializeField, HighlightIfNull, Comment("スキル説明のエリア")] private Text _skillDescription;
    [SerializeField, HighlightIfNull, Comment("解放コストのエリア")] private Text _point;
    
    private CanvasGroup _canvasGroup;
    public event Action OnGameStart;
        
    public override UniTask OnUIInitialize()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        foreach (var skillTree in _skillTrees)
        {
            skillTree.SetUIController(this);
        }
        
        SkillTextsUpdate(" ", " ", " ");
        
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
