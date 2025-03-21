using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// スキルツリー スキルボタン用の拡張
/// </summary>
[CustomEditor(typeof(SkillButton))]
public class SkillButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        SkillButton skillButton = (SkillButton)target;
        GUI.backgroundColor = Color.red;
        
        if (skillButton.SkillDataCheck())
        {
            if (GUILayout.Button("スキルデータを自動作成＆アサイン"))
            {
                CreateAndAssignSkillData(skillButton);
            }
        }
    }
    
    /// <summary>
    /// スキルデータSOを自動作成しアサインする
    /// </summary>
    public void CreateAndAssignSkillData(SkillButton skillButton)
    {
        string folderPath = "Assets/Data/SkillData/";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ファイル名をユニークIDで作成
        string skillName = skillButton.gameObject.name;
        string assetPath = $"{folderPath}{skillName}.asset";

        // ScriptableObject を新規作成
        SkillDataSO newSkillData = CreateInstance<SkillDataSO>();
        newSkillData.name = skillButton.gameObject.name; // スキルの初期名をオブジェクト名にする
        
        // アセットとして保存
        AssetDatabase.CreateAsset(newSkillData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 自動アサイン
        skillButton.SetSkillData(newSkillData);
        serializedObject.ApplyModifiedProperties();
        
        // インスペクタを更新
        EditorUtility.SetDirty(skillButton);
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.Repaint();
        }
        Repaint();
        
        CreateNewEnum(skillButton.gameObject.name);

        Debug.Log($"スキルデータを作成しました: {assetPath}");
    }

    private void CreateNewEnum(string enumName)
    {
        // 既存のenumファイルを読み込む
        string enumFilePath = "Assets/Script/SkillTree/SkillEnum.cs";
        string enumFileContent = File.ReadAllText(enumFilePath);
        
        // enumの開始部分を探す
        string enumPattern = @"public enum SkillEnum\s*\{";
        Regex enumRegex = new Regex(enumPattern);
        
        if (enumRegex.IsMatch(enumFileContent))
        {
            // enum内の最後の部分を特定
            string enumContent = enumFileContent.Substring(enumFileContent.IndexOf("{") + 1, enumFileContent.LastIndexOf("}") - enumFileContent.IndexOf("{") - 1);
            string newEnumValues = "";

            // スクリプタブルオブジェクトの名前をenumの値として追加
            enumName = enumName.Replace(" ", "").Replace("-", "_");  // スペースやハイフンを置換
            if (!enumContent.Contains(enumName)) // 既に同じ名前がないか確認
            {
                newEnumValues += $"    {enumName},";
            }

            // 新しい値を追加して、ファイルを更新
            if (!string.IsNullOrEmpty(newEnumValues))
            {
                string updatedEnumContent = enumFileContent.Replace("{", "{\n" + newEnumValues);
                File.WriteAllText(enumFilePath, updatedEnumContent);

                // 更新を通知
                AssetDatabase.Refresh();
                Debug.Log("SkillEnum の更新が完了しました！");
            }
            else
            {
                Debug.Log("SkillEnum が追加できませんでした");
            }
        }
        else
        {
            Debug.LogError("SkillEnum ファイルが見つかりませんでした");
        }
    }
}
