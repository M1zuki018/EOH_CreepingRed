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
    private string _outputUIDataPath = "Assets/Data/AreaViewData/"; // View用のデータを出力する先のファイル
    private GameObject _areaSetObject; // AreaSettingsを登録したいオブジェクト
    
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
        _outputUIDataPath = EditorGUILayout.TextField("UI用データの出力フォルダ", _outputUIDataPath);
        _areaSetObject = (GameObject)EditorGUILayout
            .ObjectField("AreaSettingsを登録したいオブジェクト", _areaSetObject, typeof(GameObject), true);
        
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

        ClearAndCreateOutputDirectories();

        string[] lines = File.ReadAllLines(_csvFilePath);
        List<AreaSettingsSO> createdAreas = new List<AreaSettingsSO>();
        List<AreaViewSettingsSO> createdUIAreas = new List<AreaViewSettingsSO>();
        
        // 縦横共にヘッダー行列があるので、その部分は読み飛ばす。19エリア分を処理
        for (int i = 1; i < 1 + AreaCount; i++)
        {
            ProcessAreaData(lines, i, createdAreas, createdUIAreas);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        AssignAreasToSimulator(createdAreas); // 自動アサイン
        AssignAreasToSimulator(createdUIAreas);

        Debug.Log("ScriptableObjectの生成が完了しました！");
    }
    
    /// <summary>
    /// 出力先のディレクトリをクリアし再作成する
    /// </summary>
    private void ClearAndCreateOutputDirectories()
    {
        if (Directory.Exists(_outputPath))　Directory.Delete(_outputPath, true); //実行用データフォルダ
        if (Directory.Exists(_outputUIDataPath))　Directory.Delete(_outputUIDataPath, true); // UI用データフォルダ
        Directory.CreateDirectory(_outputPath);
        Directory.CreateDirectory(_outputUIDataPath);
    }

    /// <summary>
    /// CSVデータを処理してScriptableObjectを生成
    /// </summary>
    private void ProcessAreaData(string[] lines, int i, List<AreaSettingsSO> createdAreas, List<AreaViewSettingsSO> createdUIAreas)
    {
        string[] values = lines[i].Split(',');
            
        if (values.Length < 15)
        {
            Debug.LogWarning($"CSVデータが不足しています (行 {i})");
            return;
        }

        try
        {
            // 実行用ScriptableObjectの生成
            var areaSettings = CreateAreaSettingsSO(values);
            string assetPath = $"{_outputPath}{areaSettings.Name.ToString()}.asset";
            AssetDatabase.CreateAsset(areaSettings, assetPath);
            createdAreas.Add(areaSettings);
                
            // UI用ScriptableObjectの生成
            var areaViewSettings = AreaViewSettingsSO(values);
            string uiAssetPath = $"{_outputUIDataPath}{areaViewSettings.Name.ToString()}.asset";
            AssetDatabase.CreateAsset(areaViewSettings, uiAssetPath);
            createdUIAreas.Add(areaViewSettings);
        }
        catch (Exception ex) when (ex is FormatException || ex is OverflowException)
        {
            Debug.LogError($"CSVデータの変換エラー (行 {i}): {ex.Message}");
        }
    }

    /// <summary>
    /// AreaSettingsSOを生成
    /// </summary>
    private AreaSettingsSO CreateAreaSettingsSO(string[] values)
    {
        AreaSettingsSO areaSettings = CreateInstance<AreaSettingsSO>();
        areaSettings.X = int.Parse(values[1]);
        areaSettings.Y = int.Parse(values[2]);
        areaSettings.Name = ExtensionsUtility.ToSectionEnum(values[3]);
        areaSettings.Explaination = values[4];
        areaSettings.Category = ExtensionsUtility.ToCategoryEnum(values[5]);
        areaSettings.Population = int.Parse(values[6]);
        areaSettings.CitizenPopulation = int.Parse(values[7]);
        areaSettings.MagicSoldierPopulation = int.Parse(values[8]);
        areaSettings.AreaSize = float.Parse(values[9]);
        areaSettings.PopulationDensity = int.Parse(values[10]);
        areaSettings.Security = int.Parse(values[11]);
        areaSettings.MobilityRate = int.Parse(values[12]);
        areaSettings.InfectionRate = int.Parse(values[13]);
        areaSettings.Control = int.Parse(values[14]);
        return areaSettings;
    }

    /// <summary>
    /// AreaViewSettingsSOを作成
    /// </summary>
    private AreaViewSettingsSO AreaViewSettingsSO(string[] values)
    {
        AreaViewSettingsSO areaViewSettings = CreateInstance<AreaViewSettingsSO>();
        areaViewSettings.Name = ExtensionsUtility.ToSectionEnum(values[3]);
        areaViewSettings.Explaination = values[4];
        areaViewSettings.Category = ExtensionsUtility.ToCategoryEnum(values[5]);
        return areaViewSettings;
    }

    /// <summary>
    /// SimulatorクラスのAreaSettingsのリストに自動アサインする
    /// </summary>
    private void AssignAreasToSimulator(List<AreaViewSettingsSO> createdAreas)
    {
        InGameSceneUIManager uiManager = FindObjectOfType<InGameSceneUIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("シーン内にInGameSceneUIManagerが見つかりません！");
            return;
        }
        uiManager.RegisterAreas(createdAreas);
        Debug.Log($"UIManagerに {createdAreas.Count} 個の区域を登録しました。");
    }
    
    /// <summary>
    /// SimulatorクラスのAreaSettingsのリストに自動アサインする
    /// </summary>
    private void AssignAreasToSimulator(List<AreaSettingsSO> createdAreas)
    {
        ISimulator simulator = _areaSetObject.GetComponent<ISimulator>();
        
        if (simulator == null)
        {
            Debug.LogWarning("シミュレーターが適切に取得できませんでした");
            return;
        }
        
        if (simulator is UITestSimulator uiTestSimulator)
        {
            uiTestSimulator.RegisterAreas(createdAreas);
        }
        else if (simulator is TestSimulator testSimulator)
        {
            testSimulator.RegisterAreas(createdAreas);
        }
        
        Debug.Log($"Simulatorに {createdAreas.Count} 個の区域を登録しました。");
    }
}
