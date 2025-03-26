using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 感染のテキストを管理するクラス
/// </summary>
public class InfectionCountText : ViewBase
{
    [SerializeField, HighlightIfNull] private Text _healthy;
    [SerializeField, HighlightIfNull] private Text _infected;
    [SerializeField, HighlightIfNull] private Text _nearDeath;

    public override UniTask OnUIInitialize()
    {
        _healthy.text = "";
        _infected.text = "";
        _nearDeath.text = "";
        
        return base.OnUIInitialize();
    }
}
