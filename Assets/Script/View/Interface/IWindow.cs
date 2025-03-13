using UnityEngine;

/// <summary>
/// UI画面が継承するインターフェース
/// </summary>
public interface IWindow
{
    public void Show();
    public void Hide();
    public void Block();
}
