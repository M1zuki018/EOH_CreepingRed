using UnityEditor;
using UnityEngine;

/// <summary>
/// オブジェクトが未割当の場合プロパティの背景色を変更する
/// </summary>
[CustomPropertyDrawer(typeof(HighlightIfNullAttribute))]
public class HighlightIfNullDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // ObjectReference型で、値がnullの場合に背景色を変更
        bool isNull = property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null;

        Color defaultColor = GUI.backgroundColor; // デフォルトの背景色を保存

        if (isNull)
        {
            GUI.backgroundColor = Color.red; // 未割り当ての場合、背景色を赤に変更
        }
        
        EditorGUI.PropertyField(position, property, label); // プロパティを描画
        GUI.backgroundColor = defaultColor; // 背景色を元に戻す
    }
}