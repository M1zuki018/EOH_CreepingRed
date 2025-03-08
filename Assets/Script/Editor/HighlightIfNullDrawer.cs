using UnityEditor;
using UnityEngine;

/// <summary>
/// オブジェクトが未割当の場合、プロパティの背景色を変更する
/// </summary>
[CustomPropertyDrawer(typeof(HighlightIfNullAttribute))]
public class HighlightIfNullDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Color defaultColor = GUI.backgroundColor; // デフォルトの背景色を保存
        
        bool isUnassigned = property.propertyType switch // 未割り当て（null）かどうかをチェック
        {
            SerializedPropertyType.ObjectReference => property.objectReferenceValue == null,
            _ => false
        };

        // 未割り当てなら背景色を変更
        if (isUnassigned)
        {
            GUI.backgroundColor = Color.red;
        }
        
        EditorGUI.PropertyField(position, property, label); // プロパティを描画
        GUI.backgroundColor = defaultColor; // 背景色を元に戻す
    }
}