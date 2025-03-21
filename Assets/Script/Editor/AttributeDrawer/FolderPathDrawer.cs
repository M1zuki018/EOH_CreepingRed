using UnityEditor;
using UnityEngine;

/// <summary>
/// inspectorからフォルダーを開き、パスを直接指定できるようにします
/// </summary>
[CustomPropertyDrawer(typeof(FolderPathAttribute))]
public class FolderPathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            float buttonWidth = 80f; // ボタンの幅

            // ラベルを描画（TextField に直接ラベルを設定しない）
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Rect textFieldRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth - buttonWidth - 5, position.height);
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            // プロパティ編集の範囲を開始
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(labelRect, label); // ラベルを別途描画

            EditorGUI.BeginChangeCheck(); // 変更を検知
            string newValue = EditorGUI.TextField(textFieldRect, property.stringValue); // label を削除

            if (GUI.Button(buttonRect, "Select"))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");

                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (folderPath.StartsWith(Application.dataPath))
                    {
                        folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
                    }
                    newValue = folderPath;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = newValue;
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty(); // ここでエラーが出なくなる
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [FolderPath] with string.");
        }
    }
}