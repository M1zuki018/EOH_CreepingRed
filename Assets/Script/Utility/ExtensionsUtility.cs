using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 一部enumを日本語に変換する静的クラス
/// </summary>
public static class ExtensionsUtility
{
    private static readonly Dictionary<SectionEnum, string> SectionDictionary = new Dictionary<SectionEnum, string>
    {
        {SectionEnum.MagicTechnologyResearchArea, "魔法技術研究区"},
            {SectionEnum.MagicianTrainingCenter, "魔法士訓練所"},
            {SectionEnum.CentralTower, "セントラル・タワー"},
            {SectionEnum.UpperClassResidentialDistrict, "上級階級用邸宅街"},
            {SectionEnum.BaysideWarehouse, "湾岸倉庫"},
            {SectionEnum.LuxuryResidentialDistrictForTheSpecialClass, "特別階級用高級居住区"},
            {SectionEnum.MiddleClassNorthDistrict, "中流階級北区"},
            {SectionEnum.CentralTransportHub, "セントラル・トランスポートハブ"},
            {SectionEnum.AltriaArena, "アルテリア・アリーナ"},
            {SectionEnum.BioResearchCenter, "バイオ・リサーチセンター"},
            {SectionEnum.UtilityManagementCenter, "ユーティリティ管理センター"},
            {SectionEnum.MiddleClassSouthDistrict, "中流階級南区"},
            {SectionEnum.OrderSpiral, "オーダー・スパイラル"},
            {SectionEnum.OrderBazaar, "オーダー・バザール"},
            {SectionEnum.Hollow, "ホロウ"},
            {SectionEnum.WaterCycleManagementPlant, "水循環管理プラント"},
            {SectionEnum.AltriaAgriculturalDome, "アルテリア農業ドーム"},
            {SectionEnum.AltriaDistributionCenter, "アルテリア配給センター"},
            {SectionEnum.CivilianMagicTrainingCenter, "民間人魔法訓練所"},
            {SectionEnum.Sea, "海"},
    };
    
    private static readonly Dictionary<AreaCategoryEnum, string> AreaCategoryDictinary = new Dictionary<AreaCategoryEnum, string>
    {
        {AreaCategoryEnum.ArcanaEmpireManagementDistrict, "アルカナ・エンパイア管理区"},
        {AreaCategoryEnum.SeniorClassifiedZone, "上級階級特区"},
        {AreaCategoryEnum.ResidentialDistrict, "居住区"},
        {AreaCategoryEnum.CentralDistrict, "中央支配区"},
        {AreaCategoryEnum.EnvironmentalManagementZone, "環境管理区"},
        {AreaCategoryEnum.DisruptionZone, "治安崩壊区域"},
    };

    /// <summary>
    /// 区域名のenumを日本語に変換する
    /// </summary>
    public static string SectionEnumToJapanese(SectionEnum sectionEnum)
    {
        return SectionDictionary.TryGetValue(sectionEnum, out var name) ? name : sectionEnum.ToString();
    }

    /// <summary>
    /// 区域名の日本語をenumに変換する
    /// </summary>
    public static SectionEnum ToSectionEnum(string sectionName)
    {
        return SectionDictionary.FirstOrDefault(kvp => kvp.Value == sectionName).Key;
    }

    /// <summary>
    /// エリアカテゴリの日本語をenumに変換する
    /// </summary>
    public static AreaCategoryEnum ToCategoryEnum(string areaCategoryName)
    {
        return AreaCategoryDictinary.FirstOrDefault(kvp => kvp.Value == areaCategoryName).Key;
    }
}
