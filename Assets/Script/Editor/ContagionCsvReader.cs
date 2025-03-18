using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 感染スキルデータのcsvファイルを読み込み
/// 対応するScriptableObjectにデータを自動入力する拡張
/// </summary>
public class ContagionCsvReader : EditorWindow
{
    private string _csvFilePath = "Assets/Data/CSV/ContagionData.csv"; // csvファイルのパス
    private string _searchFilePath = "Assets/Data/SkillData/"; // 検索を行うファイルのパス

    [MenuItem("Creeping Red/ContagionCsvReader")]
    public static void ShowWindow()
    {
        GetWindow<ContagionCsvReader>("ContagionCsvReader");
    }

    private void OnGUI()
    {
        GUILayout.Label("感染スキルデータのcsvファイルを読み込み\n" +
                        "対応するScriptableObjectにデータを自動入力する", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        _csvFilePath = EditorGUILayout.TextField("CSVファイルのパス", _csvFilePath);
        _searchFilePath = EditorGUILayout.TextField("ScriptableObjectの検索を行うファイルのパス", _searchFilePath);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("ScriptableObjectに自動入力"))
        {
            ImportSkillData();
        }
    }

    /// <summary>
    /// スキルデータをImportする
    /// </summary>
    private void ImportSkillData()
    {
        if (!File.Exists(_csvFilePath))
        {
            Debug.LogWarning($"CSVファイルが見つかりません : {_csvFilePath}");
            return;
        }
        
        string[] lines = File.ReadAllLines(_csvFilePath);
        for(int i = 1; i < lines.Length; i++) // ヘッダー行は飛ばす
        {
            // 分割
            string[] values = lines[i].Split(',');
            
            // スキル名に基づいてスクリプタブルオブジェクトを検索
            SkillDataSO skillData = FindSkillDataByName(values[0]);

            if (skillData == null)
            {
                Debug.LogWarning($"{values[0]} のScriptableObjectデータが見つかりませんでした");
                continue;
            }
            
            skillData.SetData(values[1], values[2], int.Parse(values[3]), 
                float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]));
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Skillデータの自動入力が完了しました！");
    }

    /// <summary>
    /// スキルデータを探す
    /// </summary>
    private static SkillDataSO FindSkillDataByName(string skillName)
    {
        string searchName = skillName.Replace(" ", ""); // スペースを削除した検索名

        // "Assets/Data/SkillData/" 直下の SkillDataSO をすべて取得
        string[] assetGuids = AssetDatabase.FindAssets("t:SkillDataSO", new[] { "Assets/Data/SkillData" });

        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            SkillDataSO skillData = AssetDatabase.LoadAssetAtPath<SkillDataSO>(assetPath);

            // スペース削除後の名前と一致するものを探す
            if (skillData != null && skillData.name.Replace(" ", "") == searchName)
            {
                return skillData;
            }
        }

        return null; // 該当データがなかったらnullを返す
    }
}
