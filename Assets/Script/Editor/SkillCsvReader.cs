using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// スキルデータの入力補助をする拡張
/// </summary>
public class SkillCsvReader : EditorWindow
{
    private string _csvFilePath = "Assets/Data/CSV/ContagionData.csv"; // csvファイルのパス
    private string _searchFilePath = "Assets/Data/SkillData/"; // 検索を行うファイルのパス
    private GameObject _prefabToCreate; // 自動生成するスキルボタンのPrefab
    private GameObject _parentObject; // 生成の親オブジェクト

    [MenuItem("Creeping Red/SkillCsvReader")]
    public static void ShowWindow()
    {
        GetWindow<SkillCsvReader>("SkillCsvReader");
    }

    private void OnGUI()
    {
        GUILayout.Label("スキルデータ入力補助をする拡張", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        GUILayout.Label("csvデータをScriptableObject.nameをもとにデータを自動入力する");
        
        EditorGUILayout.Space();
        
        _csvFilePath = EditorGUILayout.TextField("CSVファイルのパス", _csvFilePath);
        _searchFilePath = EditorGUILayout.TextField("ScriptableObjectの検索を行うファイルのパス", _searchFilePath);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("ScriptableObjectに自動入力"))
        {
            ImportSkillData();
        }
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Prefab配置から全て自動で行う");
        
        EditorGUILayout.Space();
        
        _prefabToCreate = (GameObject)EditorGUILayout
            .ObjectField("Prefab to Create", _prefabToCreate, typeof(GameObject), false);
        _parentObject = (GameObject)EditorGUILayout
            .ObjectField("Parent Object", _parentObject, typeof(GameObject), true); // SceneObejectを選択できるように
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("スキルボタンを自動作成する"))
        {
            CreateSkillButtons();
        }
    }

    /// <summary>
    /// スキルボタンを自動作成する
    /// </summary>
    private void CreateSkillButtons()
    {
        if (string.IsNullOrEmpty(_csvFilePath) || _prefabToCreate == null || _parentObject == null)
        {
            Debug.LogError("CSVファイルのパス/生成するPrefab/親オブジェクトの設定が完了していません");
            return;
        }
        
        SkillButtonEditor skillButtonEditor = new SkillButtonEditor();
        string[] lines = File.ReadAllLines(_csvFilePath);

        for (int i = 2; i < lines.Length; i++) // ヘッダー行は読み飛ばす
        {
            // 分割
            string[] values = lines[i].Split(',');
            
            GameObject skillButton = Instantiate(_prefabToCreate, _parentObject.transform); // Prefabを作成
            skillButton.name = values[0];
            
            // スクリプタブルオブジェクト自動作成と自動アサイン
            SkillButton skillButtonScript = skillButton.GetComponent<SkillButton>();
            skillButtonEditor.CreateAndAssignSkillData(skillButtonScript);
            
            // データをImport
            ImportSkillData();
            
            // リストに自動アサイン
            SkillBase skillBase = _parentObject.GetComponent<SkillBase>();
            if (skillBase != null)
            {
                skillBase.SkillButtons.Add(skillButtonScript);
            }
            else
            {
                Debug.LogError("親オブジェクトに指定されたオブジェクトからSkillBaseが取得できませんでした");
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("スキルボタンの作成が完了しました！");
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
            
            if(values.Length > 7) continue;
            
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
