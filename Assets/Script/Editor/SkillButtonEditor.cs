using System.IO;
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
        
        if (skillButton.SkillData == null)
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
    private void CreateAndAssignSkillData(SkillButton skillButton)
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

        Debug.Log($"スキルデータを作成しました: {assetPath}");
    }
}
