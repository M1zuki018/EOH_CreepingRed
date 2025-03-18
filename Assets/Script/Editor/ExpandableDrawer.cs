using UnityEditor;
using UnityEngine;

/// <summary>
/// ScriptableObject を Inspector 上で展開して編集できるようにします
/// </summary>
[CustomPropertyDrawer(typeof(ExpandableAttribute))]
public class ExpandableDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null) return EditorGUIUtility.singleLineHeight;

        SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
        SerializedProperty iterator = serializedObject.GetIterator();
        iterator.NextVisible(true); // スクリプトの表示をスキップする

        float height = EditorGUIUtility.singleLineHeight + 4; // 上のオブジェクトフィールドの高さ
        while (iterator.NextVisible(false))
        {
            height += EditorGUI.GetPropertyHeight(iterator, true) + 2;
        }

        return height + 6; // 余白を追加
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // ScriptableObject のオブジェクトフィールド
        Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, label, property.objectReferenceValue, fieldInfo.FieldType, false);

        if (property.objectReferenceValue != null)
        {
            SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true); // スクリプトの表示をスキップする

            // 枠を描画
            Rect boxRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight - 4);
            GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);

            float yOffset = position.y + EditorGUIUtility.singleLineHeight + 6;
            EditorGUI.indentLevel++;

            while (iterator.NextVisible(false))
            {
                float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                Rect propRect = new Rect(position.x + 6, yOffset, position.width - 12, propHeight);
                EditorGUI.PropertyField(propRect, iterator, true);
                yOffset += propHeight + 2;
            }

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}