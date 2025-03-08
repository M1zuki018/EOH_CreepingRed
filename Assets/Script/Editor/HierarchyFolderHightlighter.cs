using UnityEditor;
using UnityEngine;

/// <summary>
/// ヒエラルキービューで#Folderで始まるオブジェクトの背景色を変更する
/// </summary>
[InitializeOnLoad]
public static class HierarchyFolderHighlighter
{
    static HierarchyFolderHighlighter()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj != null && obj.name.StartsWith("#Folder")) // "#Folder"で始まるオブジェクトを対象
        {
            EditorGUI.DrawRect(selectionRect, new Color(0.7f, 0f, 0f)); // 背景色
            
            // "#Folder" を非表示にする
            string displayName = obj.name.Replace("#Folder", "").Trim();
            
            //名前を白色で表示
            EditorGUI.LabelField(selectionRect, displayName, new GUIStyle() { normal = new GUIStyleState() { textColor = Color.white } });
        }
    }
}