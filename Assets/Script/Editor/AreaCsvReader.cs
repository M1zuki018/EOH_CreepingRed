using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 区域データをcsvファイルからスクリプタブルオブジェクトに変換する拡張
/// </summary>
public class AreaCsvReader : EditorWindow
{
    private const int AreaCount = 19;
    private string _csvFilePath = "Assets/Data/CSV/AreaData.csv"; // csvファイルのパス
    private string _outputPath = "Assets/Data/AreaData/"; // スクリプタブルオブジェクトを出力する先のファイル

    [MenuItem("Creeping Red/AreaCsvReader")]
    public static void ShowWindow()
    {
        GetWindow<AreaCsvReader>("AreaScvReader");
    }

    private void OnGUI()
    {
        GUILayout.Label("区域データcsvファイルからScriptableObjectを生成", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        
        _csvFilePath = EditorGUILayout.TextField("CSVファイルのパス", _csvFilePath);
        _outputPath = EditorGUILayout.TextField("出力フォルダ", _outputPath);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("ScriptableObjectを生成"))
        {
            CreateScriptableObjectsFromCSV();
        }
    }

    /// <summary>
    /// ファイルを変換するメソッド
    /// </summary>
    private void CreateScriptableObjectsFromCSV()
    {
        if (!File.Exists(_csvFilePath))
        {
            Debug.LogWarning($"CSVファイルが見つかりません : {_csvFilePath}");
            return;
        }

        if (Directory.Exists(_outputPath))
        {
            Directory.Delete(_outputPath, true); // フォルダが既に存在したら一旦全て削除
        }
        
        Directory.CreateDirectory(_outputPath); // フォルダを再作成
        
        string[] lines = File.ReadAllLines(_csvFilePath);
        List<AreaSettingsSO> createdAreas = new List<AreaSettingsSO>();
        
        // 縦横共にヘッダー行列があるので、その部分は読み飛ばす
        // 19エリア分を処理
        for (int i = 1; i < 1 + AreaCount; i++)
        {
            string[] values = lines[i].Split(',');
            
            if (values.Length < 15)
            {
                Debug.LogWarning($"CSVデータが不足しています (行 {i})");
                continue;
            }

            try
            {
                AreaSettingsSO areaSettings = CreateInstance<AreaSettingsSO>();
                areaSettings.X = int.Parse(values[1]);
                areaSettings.Y = int.Parse(values[2]);
                areaSettings.Name = ConversionSectionName(values[3]);
                areaSettings.Explaination = values[4];
                areaSettings.Category = ConversionCategoryName(values[5]);
                areaSettings.Population = int.Parse(values[6]);
                areaSettings.CitizenPopulation = int.Parse(values[7]);
                areaSettings.MagicSoldierPopulation = int.Parse(values[8]);
                areaSettings.AreaSize = float.Parse(values[9]);
                areaSettings.PopulationDensity = int.Parse(values[10]);
                areaSettings.Security = int.Parse(values[11]);
                areaSettings.MobilityRate = int.Parse(values[12]);
                areaSettings.InfectionRate = int.Parse(values[13]);
                areaSettings.Control = int.Parse(values[14]);

                string assetPath = $"{_outputPath}{areaSettings.Name.ToString()}.asset";
                AssetDatabase.CreateAsset(areaSettings, assetPath);
                createdAreas.Add(areaSettings);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                Debug.LogError($"CSVデータの変換エラー (行 {i}): {ex.Message}");
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssignAreasToSimulator(createdAreas); // 自動アサイン

        Debug.Log("ScriptableObjectの生成が完了しました！");
    }

    /// <summary>
    /// SimulatorクラスのAreaSettingsのリストに自動アサインする
    /// </summary>
    private void AssignAreasToSimulator(List<AreaSettingsSO> createdAreas)
    {
        TestSimulator simulator = FindObjectOfType<TestSimulator>();
        if (simulator == null)
        {
            Debug.LogWarning("シーン内にSimulatorが見つかりません！");
            return;
        }
        simulator.RegisterAreas(createdAreas);
        Debug.Log($"Simulatorに {createdAreas.Count} 個の区域を登録しました。");
    }

    /// <summary>
    /// 日本語区域名をEnumに変換する
    /// </summary>
    private SectionEnum ConversionSectionName(string sectionName)
    {
        SectionEnum sectionEnum = sectionName switch
        {
            "魔法技術研究区" => SectionEnum.MagicTechnologyResearchArea,
            "魔法士訓練所" => SectionEnum.MagicianTrainingCenter,
            "セントラル・タワー" => SectionEnum.CentralTower,
            "上級階級用邸宅街" => SectionEnum.UpperClassResidentialDistrict,
            "湾岸倉庫" => SectionEnum.BaysideWarehouse,
            "特別階級用高級居住区" => SectionEnum.LuxuryResidentialDistrictForTheSpecialClass,
            "中流階級北区" => SectionEnum.MiddleClassNorthDistrict,
            "セントラル・トランスポートハブ" => SectionEnum.CentralTransportHub,
            "アルテリア・アリーナ" => SectionEnum.AltriaArena,
            "バイオ・リサーチセンター" => SectionEnum.BioResearchCenter,
            "ユーティリティー管理センター" => SectionEnum.UtilityManagementCenter,
            "中流階級南区" => SectionEnum.MiddleClassSouthDistrict,
            "オーダー・スパイラル" => SectionEnum.OrderSpiral,
            "オーダー・バザール" => SectionEnum.OrderBazaar,
            "ホロウ" => SectionEnum.Hollow,
            "水循環管理プラント" => SectionEnum.WaterCycleManagementPlant,
            "アルテリア農業ドーム" => SectionEnum.AltriaAgriculturalDome,
            "アルテリア配給センター" => SectionEnum.AltriaDistributionCenter,
            "民間人魔法訓練所" => SectionEnum.CivilianMagicTrainingCenter,
            "海" => SectionEnum.Sea,
            _ => throw new ArgumentException($"無効な区域名: {sectionName}") // 例外処理
        }; 
        return sectionEnum;
    }

    /// <summary>
    /// 日本語区域名をEnumに変換する
    /// </summary>
    private AreaCategoryEnum ConversionCategoryName(string categoryName)
    {
        AreaCategoryEnum areaCategoryEnum = categoryName switch
        {
            "アルカナ・エンパイア管理区" => AreaCategoryEnum.ArcanaEmpireManagementDistrict,
            "上級階級特区" => AreaCategoryEnum.SeniorClassifiedZone,
            "居住区" => AreaCategoryEnum.ResidentialDistrict,
            "中央支配区" => AreaCategoryEnum.CentralDistrict,
            "環境管理区" => AreaCategoryEnum.EnvironmentalManagementZone,
            "治安崩壊区域" => AreaCategoryEnum.DisruptionZone,
            _ => throw new ArgumentException($"無効な区域名: {categoryName}") // 例外処理
        };
        
        return areaCategoryEnum;
    }
}
