using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Titleシーン難易度選択画面のUIController
/// ルール：
/// ①Prefabを越えた参照はとらない
/// ②Unityとの連結を担当
/// ③具体的な処理は上位のManagerクラスに任せる
/// </summary>
public class DifficultySelectionUIController : UIControllerBase
{
    [SerializeField, HighlightIfNull] private Button[] _difficultyButtons = new Button[5];
    
    public event Action OnSelect;

    protected override void RegisterEvents()
    {
        for (int i = 0; i < _difficultyButtons.Length; i++)
        {
            DifficultyEnum difficulty = (DifficultyEnum)i; // Enum値を取得
            _difficultyButtons[i].onClick.AddListener(() => Registration(difficulty));
        }
    }

    protected override void UnregisterEvents()
    {
        for (int i = 0; i < _difficultyButtons.Length; i++)
        {
            DifficultyEnum difficulty = (DifficultyEnum)i; // Enum値を取得
            _difficultyButtons[i].onClick.RemoveListener(() => Registration(difficulty));
        }
    }
    
    private void Registration(DifficultyEnum difficulty)
    {
        GameSettingsManager.Difficulty = difficulty; // 難易度をセット
        OnSelect?.Invoke();
    }
    
    public override void Show() => CanvasVisibilityUtility.Show(_canvasGroup);
    public override void Hide() => CanvasVisibilityUtility.Hide(_canvasGroup);
    public override void Block() => CanvasVisibilityUtility.Block(_canvasGroup);
}
