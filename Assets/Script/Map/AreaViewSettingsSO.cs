using UnityEngine;

/// <summary>
/// エリアのUI表示用の設定情報を保持するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "AreaViewSettings", menuName = "Create SO/AreaViewSettings")]
public class AreaViewSettingsSO : ScriptableObject
{
    public SectionEnum Name; // 名称
    public string Explaination; // 区域の概要
    public AreaCategoryEnum Category; // 区域の区分
    public Sprite Background;
}
